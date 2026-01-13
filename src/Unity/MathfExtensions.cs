using UnityEngine;


namespace WrathTools.Unity
{
  public static class MathfExtensions
  {

    /// <summary> Returns the value multiplied by the Mathf defined const <see cref="Mathf.Rad2Deg"/> </summary>
    public static float ToDeg(this float value) => value * Mathf.Rad2Deg;
    /// <summary> Returns the value multiplied by the Mathf defined const <see cref="Mathf.Deg2Rad"/> </summary>
    public static float ToRad(this float value) => value * Mathf.Deg2Rad;

    /// <summary> Returns the sine of the value </summary>
    public static float Sin(this float value) => Mathf.Sin(value);
    /// <summary> Returns the cosine of the value </summary>
    public static float Cos(this float value) => Mathf.Cos(value);
    /// <summary> Returns the tangent of the value </summary>
    public static float Tan(this float value) => Mathf.Tan(value);
    /// <summary> Returns the arc-sine of the value </summary>
    public static float Asin(this float value) => Mathf.Asin(value);
    /// <summary> Returns the arc-cosine of the value </summary>
    public static float Acos(this float value) => Mathf.Acos(value);
    /// <summary> Returns the arc-tangent of the value </summary>
    public static float Atan(this float value) => Mathf.Atan(value);

    /// <summary> Returns the value clamped between 0 and 1</summary>
    public static float Clamp01(this float value) => Mathf.Clamp01(value);
    /// <summary> Returns the nearest integer greater or equal to the value </summary>
    public static float Ceil(this float value) => Mathf.Ceil(value);
    /// <summary> Returns the nearest integer less than or equal to the value </summary>
    public static float Floor(this float value) => Mathf.Floor(value);
    /// <summary> Returns the absolute value of the provided value. </summary>
    public static float Abs(this float value) => Mathf.Abs(value);

    /// <summary> Returns true if the two numbers are similar. </summary>
    public static bool Approximately(this float value, float other) => Mathf.Approximately(value, other);
    ///<summary> Returns true if all components of two Vectors are similar </summary>
    public static bool Approximately(this Vector3 value, Vector3 other)
    {
      return value.x.Approximately(other.x)
        && value.y.Approximately(other.y)
        && value.z.Approximately(other.z);
    }
    public static float Pow(this float value, float pow) => Mathf.Pow(value, pow);

  }
}