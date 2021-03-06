using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Meetingraum : InsideRoom {
    public GameObject emergencyButton;
    public override void generateInside(WorldGenerator wGen, List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        // create ground
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                wGen.CreateGrassGround(new Vector2(x, y), 0.04, 0.01, 0.1);
            }
        }

        Vector2 posEmergencyButton = new Vector2((float) innerRect.X + ((float) innerRect.Width) / 2, (float) innerRect.Y + ((float) innerRect.Height) / 2);
        emergencyButton = wGen.CreateAssetFromPrefab(posEmergencyButton, "Assets/Prefabs/emergencyButton.prefab");
        placedObjects.Add(emergencyButton);
        Rectangle centerRect = new Rectangle((int) posEmergencyButton.x - 2, (int) posEmergencyButton.y - 2, 5, 5);

        Vector2 posRune = new Vector2(0, 0);
        while (posRune.x == 0) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + 1 + random.Next(innerRect.Height - 1));
            if (IsPosFree(pos, corridors, placedObjects)) {
                posRune = pos;
            }
        }
        GameObject rune = wGen.CreateAssetFromPrefab(posRune + new Vector2(0.5f, 0), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Rune Pillar X2.prefab");
        placedObjects.Add(rune);
        task = rune;

        if (ventName != "") {
            Vector2 posVent = new Vector2(0, 0);
            while (posVent.x == 0) {
                Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + 2 + random.Next(innerRect.Height - 2));
                if (IsPosFree(pos, corridors, placedObjects)) {
                    posVent = pos;
                }
            }
            GameObject vent = wGen.CreateAssetFromPrefab(posVent + new Vector2(0.5f, 0), "Assets/Prefabs/Vent.prefab");
            placedObjects.Add(vent);
            vent.name = ventName;
        }

        // create objects at the wall
        string[] wallObjects = {
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Barrel.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Crate Small.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Crate.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot A.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot B.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot C.prefab"
        };
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x += 1 +random.Next(1)) {
            if (random.NextDouble() <= 0.5) {
                int[] ys = {innerRect.Y, innerRect.Y + innerRect.Height - 1};

                foreach (int y in ys) {
                    Vector2 pos = new Vector2(x, y);
                    if (IsPosFree(pos, corridors, placedObjects)) {
                        placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), wallObjects[random.Next(wallObjects.Length)]));
                    }
                }
            }
        }
        for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y += 1 +random.Next(1)) {
            if (random.NextDouble() <= 0.5) {
                int[] xs = {innerRect.X, innerRect.X + innerRect.Width - 1};

                foreach (int x in xs) {
                    Vector2 pos = new Vector2(x, y);
                    if (IsPosFree(pos, corridors, placedObjects)) {
                        placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), wallObjects[random.Next(wallObjects.Length)]));
                    }
                }
            }
        }

        // place random pillars
        string[] pillars = {
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pillar Broken.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pillar.prefab"
        };
        int n = innerRect.Width * innerRect.Height / 5;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (!VirtualGenRoom.IsCloserToThan(pos, centerRect, "XY", 1) && IsPosFree(pos, corridors, placedObjects)) {
                placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(pos.x + 0.5f, pos.y), pillars[random.Next(pillars.Length)]));
            }
        }
    }
}