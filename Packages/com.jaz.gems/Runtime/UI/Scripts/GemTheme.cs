// Jaz's Gems — GemTheme
// Purpose: Color palette data container for the emerald design system
// Used by: All UI components via Base.Theme

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

public class GemTheme : UdonSharpBehaviour
{
    [Header("Base")]
    public Color Light = Color.white;
    public Color Darker = Color.grey;
    public Color Dark = Color.black;

    [Header("Colors: Tints")]
    public Color Primary;
    public Color PrimaryLight;
    public Color PrimaryLightest;

    public Color Secondary;
    public Color SecondaryLight;
    public Color SecondaryLightest;

    [Header("Colors: Surfaces")]
    public Color Surface;
    public Color SurfaceLight;
    public Color SurfaceLighter;
    public Color SurfaceLightest;

    public Color SurfaceTonal;
    public Color SurfaceTonalLight;
    public Color SurfaceTonalLighter;
    public Color SurfaceTonalLightest;

    [Header("Colors: Semantic")]
    public Color Success;
    public Color SuccessLight;
    public Color SuccessLightest;

    public Color Warning;
    public Color WarningLight;
    public Color WarningLightest;

    public Color Danger;
    public Color DangerLight;
    public Color DangerLightest;

    public Color Info;
    public Color InfoLight;
    public Color InfoLightest;
}
