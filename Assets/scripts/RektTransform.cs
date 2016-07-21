/*
The MIT License (MIT)

Copyright (c) 2015 Christian 'ketura' McCarty

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

//Uncomment this line or define it in your own script to get a Debug log every time one of these functions is called.
//#define REKT_LOG_ACTIVE

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/*
This file is intended to be used to alleviate the rather obtuse nature of the RectTransform.  RectTransform
is brilliant in the inspector, but leaves a bit to be desired when accessing via code.  This reproduces
all of the inspector's functionality (and then some) and makes some of the more AnnoyingToTypeAndHardToRemember
API calls into something more user-friendly.  

Speed was not the primary concern of this library, so I would not recommend using this in extremely 
frequent position adjustments.  However, now that you have this, you can see how to replicate everything
you might need, so it doubles as a reference resource as well.
*/

namespace RektTransform
{
  public static class Anchors
  {
    //All the default anchoring positions.  Note that unlike most grid-based systems, this
    // follows the form "Y, X", in accordance with how it is spoken aloud in English.
    //Thus, "StretchLeft" means stretching along the Y axis, and left-aligned on the X.
    public static readonly MinMax TopLeft = new MinMax(0, 1, 0, 1);
    public static readonly MinMax TopCenter = new MinMax(0.5f, 1, 0.5f, 1);
    public static readonly MinMax TopRight = new MinMax(1, 1, 1, 1);
    public static readonly MinMax TopStretch = new MinMax(0, 1, 1, 1);

    public static readonly MinMax MiddleLeft = new MinMax(0, 0.5f, 0, 0.5f);
    public static readonly MinMax TrueCenter = new MinMax(0.5f, 0.5f, 0.5f, 0.5f);
    public static readonly MinMax MiddleCenter = new MinMax(0.5f, 0.5f, 0.5f, 0.5f);
    public static readonly MinMax MiddleRight = new MinMax(1, 0.5f, 1, 0.5f);
    public static readonly MinMax MiddleStretch = new MinMax(0, 0.5f, 1, 0.5f);

    public static readonly MinMax BottomLeft = new MinMax(0, 0, 0, 0);
    public static readonly MinMax BottomCenter = new MinMax(0.5f, 0, 0.5f, 0);
    public static readonly MinMax BottomRight = new MinMax(1, 0, 1, 0);
    public static readonly MinMax BottomStretch = new MinMax(0, 0, 1, 0);

    public static readonly MinMax StretchLeft = new MinMax(0, 0, 0, 1);
    public static readonly MinMax StretchCenter = new MinMax(0.5f, 0, 0.5f, 1);
    public static readonly MinMax StretchRight = new MinMax(1, 0, 1, 1);
    public static readonly MinMax TrueStretch = new MinMax(0, 0, 1, 1);
    public static readonly MinMax StretchStretch = new MinMax(0, 0, 1, 1);
  }


  //Used to store the anchors of a RectTransform.  Could potentially be used for other things.
  public struct MinMax
  {
    public Vector2 min;
    public Vector2 max;

    public MinMax(Vector2 min, Vector2 max)
    {
      this.min = new Vector2(Mathf.Clamp01(min.x), Mathf.Clamp01(min.y));
      this.max = new Vector2(Mathf.Clamp01(max.x), Mathf.Clamp01(max.y));
    }

    public MinMax(float minx, float miny, float maxx, float maxy)
    {
      this.min = new Vector2(Mathf.Clamp01(minx), Mathf.Clamp01(miny));
      this.max = new Vector2(Mathf.Clamp01(maxx), Mathf.Clamp01(maxy));
    }
  }


  public static class Cast
  {
    public static RectTransform RT(this GameObject go)
    {
      if (go == null || go.transform == null)
        return null;

      return go.GetComponent<RectTransform>();
    }

    public static RectTransform RT(this Transform t)
    {
      if (t is RectTransform == false)
        return null;

      return t as RectTransform;
    }

    public static RectTransform RT(this Component c)
    {
      return RT(c.transform);
    }

    public static RectTransform RT(this UIBehaviour ui)
    {
      if (ui == null)
        return null;

      return ui.transform as RectTransform;
    }
  }

  public static class RectTransformExtension
  {
    //If REKT_LOG_ACTIVE is defined, Debug output will come through here.  Otherwise
    // there will be nothing.
    [Conditional("REKT_LOG_ACTIVE")]
    private static void Log(object message)
    {
      UnityEngine.Debug.Log(message);
    }

    //Used for the creation of this library, but may come in handy for others.

    public static void DebugOutput(this RectTransform RT)
    {
      Log("Debug printing: " + RT);
      Log("Pos: " + RT.localPosition);
      Log("Rect: " + RT.rect);
      Log("Size Delta: " + RT.sizeDelta);
      Log("Pivot: " + RT.pivot);
      Log("Offset Min: " + RT.offsetMin + ", Offset Max: " + RT.offsetMax);
      Log("Anchored Pos: " + RT.anchoredPosition);
      Log("Anchor min: " + RT.anchorMin + ", Anchor Max: " + RT.anchorMax);
      Log("\n");
    }

    //Effectively just reformats GetWorldCorners into a Rect instead of a stupid-ass Vector3 array.

    public static Rect GetWorldRect(this RectTransform RT)
    {
      //Be wary here.  A standard Rect has the position as the upper-left corner,
      // and I think the Unity UI stuff somehow repurposes this to instead point to the
      // lower-left. I'm not 100% sure on this, but I've had some unexplained wierdnesses.
      Vector3[] corners = new Vector3[4];
      RT.GetWorldCorners(corners);
      Vector2 Size = new Vector2(corners[2].x - corners[1].x, corners[1].y - corners[0].y);
      return new Rect(new Vector2(corners[1].x, -corners[1].y), Size);
    }

    //Helper function for saving the anchors as one, instead of playing with both corners.

    public static MinMax GetAnchors(this RectTransform RT)
    {
      Log("GetAnchors called on " + RT + ".");

      return new MinMax(RT.anchorMin, RT.anchorMax);
    }

    //Helper function to restore the anchors as above.

    public static void SetAnchors(this RectTransform RT, MinMax anchors)
    {
      Log("SetAnchors called on " + RT + ".");

      RT.anchorMin = anchors.min;
      RT.anchorMax = anchors.max;
    }

    //Returns the parent of the given object as a RectTransform.

    public static RectTransform GetParent(this RectTransform RT)
    {
      Log("GetParent called on " + RT + ".");

      return RT.parent as RectTransform;
    }

    //Gets the width, height, or both.  Since these are wrappers to properties, these
    // are likely quite slower than the alternative. These are included for 
    // consistency's sake.

    public static float GetWidth(this RectTransform RT)
    {
      Log("GetWidth called on " + RT + ".");

      return RT.rect.width;
    }
    public static float GetHeight(this RectTransform RT)
    {
      Log("GetHeight called on " + RT + ".");

      return RT.rect.height;
    }

    public static Vector2 GetSize(this RectTransform RT)
    {
      Log("GetSize called on " + RT + ".");

      return new Vector2(RT.GetWidth(), RT.GetHeight());
    }

    //Sets the width, height, or both.

    public static void SetWidth(this RectTransform RT, float width)
    {
      Log("SetWidth called on " + RT + ".");

      RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetHeight(this RectTransform RT, float height)
    {
      Log("SetHeight called on " + RT + ".");

      RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetSize(this RectTransform RT, float width, float height)
    {
      Log("SetSize called on " + RT + ".");

      RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
      RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetSize(this RectTransform RT, Vector2 size)
    {
      Log("SetSize called on " + RT + ".");

      RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
      RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    //There used to be SetPos functions here.  These have been removed due to the inclusion
    // of the more clear Move and MoveFrom function family.

    //These four functions actually return the center of the edge mentioned, so
    // GetLeft gives you the center-left point, etc.  

    public static Vector2 GetLeft(this RectTransform RT)
    {
      Log("GetLeft called on " + RT + ".");

      return new Vector2(RT.offsetMin.x, RT.anchoredPosition.y);
    }

    public static Vector2 GetRight(this RectTransform RT)
    {
      Log("GetRight called on " + RT + ".");

      return new Vector2(RT.offsetMax.x, RT.anchoredPosition.y);
    }

    public static Vector2 GetTop(this RectTransform RT)
    {
      Log("GetTop called on " + RT + ".");

      return new Vector2(RT.anchoredPosition.x, RT.offsetMax.y);
    }

    public static Vector2 GetBottom(this RectTransform RT)
    {
      Log("GetBottom called on " + RT + ".");

      return new Vector2(RT.anchoredPosition.x, RT.offsetMin.y);
    }

    //Similar to setting the "Left" etc variables in the inspector.  Unlike the inspector, these
    // can be used regardless of anchor position.  Be warned, there's a reason the functionality
    // is hidden in the editor, as the behavior is unintuitive when adjusting the parent's rect.
    // If you're calling these every frame or otherwise updating frequently, shouldn't be a problem, though.
    //
    //Keep in mind that these functions all use standard directions when determining positioning; this means 
    // that unlike the inspector, positive ALWAYS means to the right/top, and negative ALWAYS means to the left/
    // bottom.  If you want true inspector functionality, use Left() and so on, below.
    //
    //E.g., SetLeftEdge(-10) will turn
    /*
        .__________.
        |          |
        |          |
        |   [ ]    |
        |          |
        |__________|
    
            into
        .__________.
        |          |
        |          |
      [       ]    |
        |          |
        |__________|

      [ ] is the RectTransform, the bigger square is the parent
    */

    public static void SetLeft(this RectTransform RT, float left)
    {
      Log("SetLeft called on " + RT + ".");

      float xmin = RT.GetParent().rect.xMin;
      float anchorFactor = RT.anchorMin.x * 2 - 1;

      RT.offsetMin = new Vector2(xmin + (xmin * anchorFactor) + left, RT.offsetMin.y);
    }

    public static void SetRight(this RectTransform RT, float right)
    {
      Log("SetRight called on " + RT + ".");

      float xmax = RT.GetParent().rect.xMax;
      float anchorFactor = RT.anchorMax.x * 2 - 1;

      RT.offsetMax = new Vector2(xmax - (xmax * anchorFactor) + right, RT.offsetMax.y);
    }

    public static void SetTop(this RectTransform RT, float top)
    {
      Log("SetTop called on " + RT + ".");

      float ymax = RT.GetParent().rect.yMax;
      float anchorFactor = RT.anchorMax.y * 2 - 1;

      RT.offsetMax = new Vector2(RT.offsetMax.x, ymax - (ymax * anchorFactor) + top);
    }

    public static void SetBottom(this RectTransform RT, float bottom)
    {
      Log("SetBottom called on " + RT + ".");

      float ymin = RT.GetParent().rect.yMin;
      float anchorFactor = RT.anchorMin.y * 2 - 1;

      RT.offsetMin = new Vector2(RT.offsetMin.x, ymin + (ymin * anchorFactor) + bottom);
    }

    //Truly matches the functionality of the "Left" etc property in the inspector. This means that
    // Right(10) will actually move the right edge to 10 units from the LEFT of the parent's right
    // edge.  In other words, all coordinates are "inside": they measure distance from the parent's
    // edge to the inside of the parent.

    public static void Left(this RectTransform RT, float left)
    {
      RT.SetLeft(left);
    }

    public static void Right(this RectTransform RT, float right)
    {
      RT.SetRight(-right);
    }

    public static void Top(this RectTransform RT, float top)
    {
      RT.SetTop(-top);
    }

    public static void Bottom(this RectTransform RT, float bottom)
    {
      RT.SetRight(bottom);
    }

    //Repositions the requested edge relative to the passed anchor. This lets you set e.g.
    // the left edge relative to the parent's right edge, etc.
    //
    //While this is intended for use with the default anchors, really arbitrary points
    // can be used.

    public static void SetLeftFrom(this RectTransform RT, MinMax anchor, float left)
    {
      Log("SetLeftFrom called on " + RT + ".");

      Vector2 origin = RT.AnchorToParentSpace(anchor.min - RT.anchorMin);

      RT.offsetMin = new Vector2(origin.x + left, RT.offsetMin.y);
    }

    public static void SetRightFrom(this RectTransform RT, MinMax anchor, float right)
    {
      Log("SetRightFrom called on " + RT + ".");

      Vector2 origin = RT.AnchorToParentSpace(anchor.max - RT.anchorMax);

      RT.offsetMax = new Vector2(origin.x + right, RT.offsetMax.y);
    }

    public static void SetTopFrom(this RectTransform RT, MinMax anchor, float top)
    {
      Log("SetTopFrom called on " + RT + ".");

      Vector2 origin = RT.AnchorToParentSpace(anchor.max - RT.anchorMax);

      RT.offsetMax = new Vector2(RT.offsetMax.x, origin.y + top);
    }

    public static void SetBottomFrom(this RectTransform RT, MinMax anchor, float bottom)
    {
      Log("SetBottomFrom called on " + RT + ".");

      Vector2 origin = RT.AnchorToParentSpace(anchor.min - RT.anchorMin);

      RT.offsetMin = new Vector2(RT.offsetMin.x, origin.y + bottom);
    }

    //Moves the edge to the requested position relative to the current position.  
    // NOTE:  using these functions repeatedly will result in unintuitive
    // behavior, since the anchored position is getting changed with each call.  
 
    public static void SetRelativeLeft(this RectTransform RT, float left)
    {
      Log("SetRelativeLeft called on " + RT + ".");

      RT.offsetMin = new Vector2(RT.anchoredPosition.x + left, RT.offsetMin.y);
    }

    public static void SetRelativeRight(this RectTransform RT, float right)
    {
      Log("SetRelativeRight called on " + RT + ".");

      RT.offsetMax = new Vector2(RT.anchoredPosition.x + right, RT.offsetMax.y);
    }

    public static void SetRelativeTop(this RectTransform RT, float top)
    {
      Log("SetRelativeTop called on " + RT + ".");

      RT.offsetMax = new Vector2(RT.offsetMax.x, RT.anchoredPosition.y + top);
    }

    public static void SetRelativeBottom(this RectTransform RT, float bottom)
    {
      Log("SetRelativeBottom called on " + RT + ".");

      RT.offsetMin = new Vector2(RT.offsetMin.x, RT.anchoredPosition.y + bottom);
    }

    //Sets the position of the RectTransform relative to the parent's Left etc side,
    // regardless of anchor setting. 
    //E.g., MoveLeft(0) will look like this:
    /*
        .__________.
        |          |
        |          |
       [|]         |
        |          |
        |__________|
    */

    public static void MoveLeft(this RectTransform RT, float left = 0)
    {
      Log("MoveLeft called on " + RT + ".");

      float xmin = RT.GetParent().rect.xMin;
      float center = RT.anchorMax.x - RT.anchorMin.x;
      float anchorFactor = RT.anchorMax.x * 2 - 1;

      RT.anchoredPosition = new Vector2(xmin + (xmin * anchorFactor) + left - (center * xmin), RT.anchoredPosition.y);
    }

    public static void MoveRight(this RectTransform RT, float right = 0)
    {
      Log("MoveRight called on " + RT + ".");

      float xmax = RT.GetParent().rect.xMax;
      float center = RT.anchorMax.x - RT.anchorMin.x;
      float anchorFactor = RT.anchorMax.x * 2 - 1;

      RT.anchoredPosition = new Vector2(xmax - (xmax * anchorFactor) - right + (center * xmax), RT.anchoredPosition.y);
    }

    public static void MoveTop(this RectTransform RT, float top = 0)
    {
      Log("MoveTop called on " + RT + ".");

      float ymax = RT.GetParent().rect.yMax;
      float center = RT.anchorMax.y - RT.anchorMin.y;
      float anchorFactor = RT.anchorMax.y * 2 - 1;

      RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, ymax - (ymax * anchorFactor) - top + (center * ymax));
    }

    public static void MoveBottom(this RectTransform RT, float bottom = 0)
    {
      Log("MoveBottom called on " + RT + ".");

      float ymin = RT.GetParent().rect.yMin;
      float center = RT.anchorMax.y - RT.anchorMin.y;
      float anchorFactor = RT.anchorMax.y * 2 - 1;

      RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, ymin + (ymin * anchorFactor) + bottom - (center * ymin));
    }

    //Moves the RectTransform to align the child left edge with the parent left edge, etc.  
    //E.g., MoveLeftInside(0) will look like this:
    /*
        .__________.
        |          |
        |          |
        [ ]        |
        |          |
        |__________|
    */

    public static void MoveLeftInside(this RectTransform RT, float left = 0)
    {
      Log("MoveLeftInside called on " + RT + ".");

      RT.MoveLeft(left + RT.GetWidth() / 2);
    }

    public static void MoveRightInside(this RectTransform RT, float right = 0)
    {
      Log("MoveRightInside called on " + RT + ".");

      RT.MoveRight(right + RT.GetWidth() / 2);
    }

    public static void MoveTopInside(this RectTransform RT, float top = 0)
    {
      Log("MoveTopInside called on " + RT + ".");

      RT.MoveTop(top + RT.GetHeight() / 2);
    }

    public static void MoveBottomInside(this RectTransform RT, float bottom = 0)
    {
      Log("MoveBottomInside called on " + RT + ".");

      RT.MoveBottom(bottom + RT.GetHeight() / 2);
    }

    //Moves the RectTransform to align the child right edge with the parent left edge, etc
    //E.g., MoveLeftOutside(0) will look like this:
    /*
        .__________.
        |          |
        |          |
      [ ]          |
        |          |
        |__________|
    */

    public static void MoveLeftOutside(this RectTransform RT, float left = 0)
    {
      Log("MoveLeftOutside called on " + RT + ".");

      RT.MoveLeft(left - RT.GetWidth() / 2);
    }

    public static void MoveRightOutside(this RectTransform RT, float right = 0)
    {
      Log("MoveRightOutside called on " + RT + ".");

      RT.MoveRight(right - RT.GetWidth() / 2);
    }

    public static void MoveTopOutside(this RectTransform RT, float top = 0)
    {
      Log("MoveTopOutside called on " + RT + ".");

      RT.MoveTop(top - RT.GetHeight() / 2);
    }

    public static void MoveBottomOutside(this RectTransform RT, float bottom = 0)
    {
      Log("MoveBottomOutside called on " + RT + ".");

      RT.MoveBottom(bottom - RT.GetHeight() / 2);
    }

    //Moves the RectTransform to the given point in parent space, considering (0, 0)
    // to be the parent's lower-left corner.

    public static void Move(this RectTransform RT, float x, float y)
    {
      RT.MoveLeft(x);
      RT.MoveBottom(y);
    }

    public static void Move(this RectTransform RT, Vector2 point)
    {
      RT.MoveLeft(point.x);
      RT.MoveBottom(point.y);
    }

    //Moves the RectTransform relative to the parent's lower-left corner, respecting
    // the RT's width and height.  See MoveLeftInside.

    public static void MoveInside(this RectTransform RT, float x, float y)
    {
      RT.MoveLeftInside(x);
      RT.MoveBottomInside(y);
    }

    public static void MoveInside(this RectTransform RT, Vector2 point)
    {
      RT.MoveLeftInside(point.x);
      RT.MoveBottomInside(point.y);
    }

    //Moves the RectTransform relative to the parent's lower-left corner, respecting
    // the RT's width and height.  See MoveLeftOutside.

    public static void MoveOutside(this RectTransform RT, float x, float y)
    {
      RT.MoveLeftOutside(x);
      RT.MoveBottomOutside(y);
    }

    public static void MoveOutside(this RectTransform RT, Vector2 point)
    {
      RT.MoveLeftOutside(point.x);
      RT.MoveBottomOutside(point.y);
    }

    //Moves the RectTransform relative to an arbitrary anchor point.  This is effectively 
    // like setting the anchor, then moving, then setting it back, but does so without
    // potentially getting in the way of anything else.

    public static void MoveFrom(this RectTransform RT, MinMax anchor, Vector2 point)
    {
      RT.MoveFrom(anchor, point.x, point.y);
    }

    public static void MoveFrom(this RectTransform RT, MinMax anchor, float x, float y)
    {
      Log("MoveFrom called on " + RT + ".");

      Vector2 origin = RT.AnchorToParentSpace(AnchorOrigin(anchor) - RT.AnchorOrigin());
      RT.anchoredPosition = new Vector2(origin.x + x, origin.y + y);
    }

    //Translates a point on the parent's frame of reference, with (0, 0) being the parent's 
    // lower-left hand corner, into the same point relative to the RectTransform's current
    // anchor. 
    
    public static Vector2 ParentToChildSpace(this RectTransform RT, Vector2 point)
    {
      return RT.ParentToChildSpace(point.x, point.y);
    }

    public static Vector2 ParentToChildSpace(this RectTransform RT, float x, float y)
    {
      float xmin = RT.GetParent().rect.xMin;
      float ymin = RT.GetParent().rect.yMin;
      float anchorFactorX = RT.anchorMin.x * 2 - 1;
      float anchorFactorY = RT.anchorMin.y * 2 - 1;

      return new Vector2(xmin + (xmin * anchorFactorX) + x, ymin + (ymin * anchorFactorY) + y);
    }


    //Translates a point (presumably the RectTransform's anchoredPosition) into the same
    // point on the parent's frame of reference, with (0, 0) being the parent's lower-left
    // hand corner.
    
    public static Vector2 ChildToParentSpace(this RectTransform RT, float x, float y)
    {
      return RT.AnchorOriginParent() + new Vector2(x, y);
    }

    public static Vector2 ChildToParentSpace(this RectTransform RT, Vector2 point)
    {
      return RT.AnchorOriginParent() + point;
    }

    //Normalizes a point associated with the parent object into "Anchor Space", which is
    // to say, (0, 0) represents the parent's lower-left-hand corner, and (1, 1) represents
    // the upper-right-hand.
    
    public static Vector2 ParentToAnchorSpace(this RectTransform RT, Vector2 point)
    {
      return RT.ParentToAnchorSpace(point.x, point.y);
    }

    public static Vector2 ParentToAnchorSpace(this RectTransform RT, float x, float y)
    {
      Rect parent = RT.GetParent().rect;
      if (parent.width != 0)
        x /= parent.width;
      else
        x = 0;

      if (parent.height != 0)
        y /= parent.height;
      else
        y = 0;

      return new Vector2(x, y);
    }

    //Translates a normalized "Anchor Space" coordinate into a real point on the parent's
    // reference system.
    
    public static Vector2 AnchorToParentSpace(this RectTransform RT, float x, float y)
    {
      return new Vector2(x * RT.GetParent().rect.width, y * RT.GetParent().rect.height);
    }

    public static Vector2 AnchorToParentSpace(this RectTransform RT, Vector2 point)
    {
      return new Vector2(point.x * RT.GetParent().rect.width, point.y * RT.GetParent().rect.height);
    }


    //Since both anchors usually sit on the same coordinate, it can be easy to treat them
    // as a single point.  This will however lead to problems whenever they are apart, such as
    // when any of the Stretch anchors are used.  This calculates the center of the rectangle
    // the two points represent, which is the origin that a RectTransform's anchoredPosition
    // is an offset of.

    public static Vector2 AnchorOrigin(this RectTransform RT)
    {
      return AnchorOrigin(RT.GetAnchors());
    }

    public static Vector2 AnchorOrigin(MinMax anchor)
    {
      float x = anchor.min.x + (anchor.max.x - anchor.min.x) / 2;
      float y = anchor.min.y + (anchor.max.y - anchor.min.y) / 2;

      return new Vector2(x, y);
    }

    //Translates a RectTransform's anchor origin into Parent space, so you don't have to pass
    // the result of AnchorOrigin() to AnchorToParentSpace().

    public static Vector2 AnchorOriginParent(this RectTransform RT)
    {
      return Vector2.Scale(RT.AnchorOrigin(), new Vector2(RT.GetParent().rect.width, RT.GetParent().rect.height));
    }

    //Helper to get the top-most-level canvas that this RectTransform is a child of.

    public static Canvas GetRootCanvas(this RectTransform RT)
    {
      Canvas rootCanvas = RT.GetComponentInParent<Canvas>();

      while (!rootCanvas.isRootCanvas)
        rootCanvas = rootCanvas.transform.parent.GetComponentInParent<Canvas>();

      return rootCanvas;
    }
  }
}

