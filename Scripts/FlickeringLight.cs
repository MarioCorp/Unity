/* ----------------------------------------------
 * 
 * Flickering Light * (C)2015 Michailidis Marios
 * 
 * ma.michailidis@gmail.com	
 * 
 * https://github.com/ToAlani
 * 
 * - Provided as is.
 * - You can change and distribute as you like.
 * 
 * ------------------------------------------ */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(Transform))]
public class FlickeringLight : MonoBehaviour {

	// Inspector Settings
	[Space(5, order=1)]
	[Header("Intensity Settings :", order=0)]
	[Range(0.0f, 8.0f)]
	public float MinimumIntensity = 0.0F;
	[Range(0.0f, 8.0f)]
	public float MaximumIntensity = 8.0F;
	public enum LightSelection {Flickering = 0, Fluorecent = 1};
	[Header("Light Type Selection :", order=0)]
	[Space(5, order=1)]
	public LightSelection LightType = LightSelection.Flickering;
	[Header("Flickering Light Settings :", order=0)]
	[Space(5, order=1)]
	[Tooltip("Checked = Random flickering between selected min & max intensity values\r\nUnChecked = Smooth flickering between intensity values")]
	[Range(0.0f, 1.0f)]
	public float FlickeringRandomness;
	public float FlickeringSpeed = 0.5f;
	public float FlickeringSpeedOffset;
	[Header("Speed Settings :", order=0)]
	[Space(5, order=1)]
	[Range(0.0f, 1.0f)]
	public float SpeedFluctuation;
	[Space(5)]
	public bool SmoothingEffect;
	[Space(5)]
	[Header("Movement Settings :")]
	public MovementSettings XAxis;
	public MovementSettings YAxis;
	public MovementSettings ZAxis;

	// Internal Settings
	Light myLight;
	Transform myTransform;
	bool xStart, xEnd;
	float xPos, xMin, xMax;
	bool yStart, yEnd;
	float yPos, yMin, yMax;
	bool zStart, zEnd;
	float zPos, zMin, zMax;

	[System.Serializable]
	public class MovementSettings{
		public bool EnableMovement;
		public float MinimumOffset;
		public float MaximumOffset;
		public float Speed;
		[Range(0.0f, 1.0f)]
		public float Randomness;
	}

	// Use this for initialization
	void Start () {
		myLight = gameObject.GetComponent<Light>();
		myTransform = GetComponent(typeof(Transform)) as Transform;

		xPos = myTransform.position.x;
		xMin = XAxis.MinimumOffset;
		xMax = XAxis.MaximumOffset;
		yPos = myTransform.position.y;
		yMin = YAxis.MinimumOffset;
		yMax = YAxis.MaximumOffset;
		zPos = myTransform.position.z;
		zMin = ZAxis.MinimumOffset;
		zMax = ZAxis.MaximumOffset;
	}

	void FixedUpdate()
	{
		LightIntensity ();
		LightMovement ();
	}

	void LightMovement()
	{
		if (XAxis.EnableMovement) {
			if (transform.position.x >= xMax)
			{
				xEnd = true;

				if (xStart && XAxis.Randomness > 0.0f)
				{
					if (Random.Range(0.0f, 1.1f) <= XAxis.Randomness)
						xMax = Random.Range(xPos, XAxis.MaximumOffset);
				}
				else if (XAxis.Randomness == 0.0f)
					xMax = XAxis.MaximumOffset;

				xStart = false;
			}
			else if (transform.position.x <= xMin)
			{
				xEnd = false;

				if (XAxis.Randomness > 0.0f && !xStart)
				{
					if (Random.Range(0.0f, 1.1f) <= XAxis.Randomness)
						xMin = Random.Range(xPos, XAxis.MinimumOffset);
				}
				else if (XAxis.Randomness == 0.0f)
					xMin = XAxis.MinimumOffset;

				xStart = true;
			}

			if (myTransform.position.x < xMax && !xEnd)
				myTransform.position = new Vector3(transform.position.x + XAxis.Speed, myTransform.position.y, myTransform.position.z);
			else
				myTransform.position = new Vector3(transform.position.x - XAxis.Speed, myTransform.position.y, myTransform.position.z);

			myTransform.position = new Vector3(
				Mathf.Clamp (myTransform.position.x, XAxis.MinimumOffset, XAxis.MaximumOffset),
				myTransform.position.y,
				myTransform.position.z
				);
		}

		if (YAxis.EnableMovement) {
			if (transform.position.y >= yMax)
			{
				yEnd = true;

				if (YAxis.Randomness > 0.0f && yStart)
				{
					if (Random.Range(0.0f, 1.1f) <= YAxis.Randomness)
						yMax = Random.Range(yPos, YAxis.MaximumOffset);
				}
				else if (YAxis.Randomness == 0.0f)
					yMax = YAxis.MaximumOffset;
				
				yStart = false;
			}
			else if (transform.position.y <= yMin)
			{
				yEnd = false;

				if (YAxis.Randomness > 0.0f && !yStart)
				{
					if (Random.Range(0.0f, 1.1f) <= YAxis.Randomness)
						yMin = Random.Range(yPos, YAxis.MinimumOffset);
				}
				else if (YAxis.Randomness == 0.0f)
					yMin = YAxis.MinimumOffset;
				
				yStart = true;
			}
			
			if (myTransform.position.y < yMax && !yEnd)
				myTransform.position = new Vector3(myTransform.position.x, transform.position.y + YAxis.Speed, myTransform.position.z);
			else
				myTransform.position = new Vector3(transform.position.x, myTransform.position.y - YAxis.Speed, myTransform.position.z);

			myTransform.position = new Vector3(
				myTransform.position.x,
				Mathf.Clamp (myTransform.position.y, YAxis.MinimumOffset, YAxis.MaximumOffset),
				myTransform.position.z
				);
		}

		if (ZAxis.EnableMovement) {
			if (transform.position.z >= zMax)
			{
				zEnd = true;

				if (ZAxis.Randomness > 0.0f && zStart)
				{
					if (Random.Range(0.0f, 1.1f) <= ZAxis.Randomness)
						zMax = Random.Range(zPos, ZAxis.MaximumOffset);
				}
				else if (ZAxis.Randomness == 0.0f)
					zMax = ZAxis.MaximumOffset;
				
				zStart = false;
			}
			else if (transform.position.z <= zMin)
			{
				zEnd = false;

				if (ZAxis.Randomness > 0.0f && !zStart)
				{
					if (Random.Range(0.0f, 1.1f) <= ZAxis.Randomness)
						zMin = Random.Range(zPos, ZAxis.MinimumOffset);
				}
				else if (ZAxis.Randomness == 0.0f)
					zMin = ZAxis.MinimumOffset;

				zStart = true;
			}
			
			if (myTransform.position.z < zMax && !zEnd)
				myTransform.position = new Vector3(transform.position.x, myTransform.position.y, myTransform.position.z + ZAxis.Speed);
			else
				myTransform.position = new Vector3(transform.position.x, myTransform.position.y, myTransform.position.z - ZAxis.Speed);

			myTransform.position = new Vector3(
				myTransform.position.x,
				myTransform.position.y,
				Mathf.Clamp (myTransform.position.z, ZAxis.MinimumOffset, ZAxis.MaximumOffset)
				);
		}
	}

	void LightIntensity()
	{
		float intentisy = 0.0f;
		
		switch (LightType) {
		case LightSelection.Flickering:
			float fluctuation = 0.0f;
			float randomSpeed = 0.0f;
			float randomFlick = 0.0f;

			if (SpeedFluctuation == 0.0f)
				fluctuation = Time.unscaledTime * FlickeringSpeed * Mathf.PI;
			else {

				if (SmoothingEffect)
				{
					randomSpeed = Mathf.Lerp(FlickeringSpeed, FlickeringSpeedOffset, 1 + SpeedFluctuation);
					fluctuation = Time.unscaledTime * randomSpeed * (1 + SpeedFluctuation);
				}
				else
				{
					randomSpeed = Random.Range (FlickeringSpeed, FlickeringSpeedOffset);
					fluctuation = Time.unscaledTime * randomSpeed * (1 + SpeedFluctuation);
				}
			}

			if (FlickeringRandomness == 0.0f)
				intentisy = Mathf.Cos (fluctuation) * (MaximumIntensity - MinimumIntensity) / 2 + (MaximumIntensity + MinimumIntensity) / 2;
			else if (Random.Range (0.0f, 1.0f) <= FlickeringRandomness) {
				randomFlick = Random.Range (MinimumIntensity, MaximumIntensity);
				intentisy = Mathf.Cos (fluctuation) * (MaximumIntensity - randomFlick) / 2 + (MaximumIntensity + randomFlick) / 2;
			} else {
				intentisy = (MaximumIntensity + MinimumIntensity) / 2;
			}
			break;
		case LightSelection.Fluorecent:
			if (Random.Range(0.0f, 1.1f) < SpeedFluctuation)
				intentisy = MinimumIntensity;
			else
				intentisy = MaximumIntensity;
			break;
		}

		myLight.intensity = intentisy;
	}
}