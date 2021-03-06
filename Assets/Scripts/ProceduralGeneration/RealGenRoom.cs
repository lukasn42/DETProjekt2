using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class RealGenRoom : GenRoom {
    public Rectangle innerRect;
    public GameObject task;
    public List<GameObject> placedObjects = new List<GameObject>();

    public string ventName = "";

    public override List<Rectangle> getRects() {
        List<Rectangle> rects = new List<Rectangle>();
        rects.Add(innerRect);
        return rects;
    }

    public override String ToString() {
        return "R";
    }

    public void generate(WorldGenerator wGen, Rectangle newOuterRect) {
        outerRect = newOuterRect;
        Rectangle innerOuterRect = new Rectangle(outerRect.X + 1, outerRect.Y + 1, outerRect.Width - 2, outerRect.Height - 2);

        int x, y, w, h;

        x = (int) Math.Ceiling((double) innerOuterRect.Width / 13 + random.Next(innerOuterRect.Width / 13));
        y = (int) Math.Ceiling((double) innerOuterRect.Height / 13 + random.Next(innerOuterRect.Height / 13));
        w = (int) Math.Ceiling((double) 11 * innerOuterRect.Width / 13 + random.Next(innerOuterRect.Width / 13) - x);
        h = (int) Math.Ceiling((double) 11 * innerOuterRect.Height / 13 + random.Next(innerOuterRect.Height / 13) - y);

        innerRect = new Rectangle(innerOuterRect.X + x, innerOuterRect.Y + y, w, h);
    }

    public static bool IsPosFree(Vector2 pos, List<Rectangle> corridors, List<GameObject> placedObjects) {
        foreach (GameObject obj in placedObjects) {
            if (VirtualGenRoom.IsCloserToThan(pos, WorldGenerator.GetRectangleFromTransform(obj.transform), "XY", 1)) {
                return false;
            }
        }
        if (VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 2)) {
            return false;
        }
        return true;
    }

    public override int getRoomCount() {
        return 1;
    }
    public abstract void generateInside(WorldGenerator wGen, List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside);
    public abstract void generateOutside(WorldGenerator wGen, List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside);
}