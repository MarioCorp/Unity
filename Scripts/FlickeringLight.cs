/* ----------------------------------------------
 * Version 3.1
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
	[Space(10)]
	[Tooltip("Checked = Random flickering between selected min & max intensity values\r\nUnChecked = Smooth flickering between intensity values")]
	[Range(0.0f, 1.0f)]
	public float FlickeringRandomness;
	public enum LightSelection {Flickering = 0, Fluorecent = 1};
	[Header("Light Type Selection :", order=0)]
	[Space(5, order=1)]
	public LightSelection LightType = LightSelection.Flickering;
	[Header("Light Speed Settings :", order=0)]
	[Space(5, order=1)]
	public float Speed = 1.0F;
	public float SpeedOffset;
	[Range(0.0f, 1.0f)]
	public float SpeedFluctuation;
	[Space(5)]
	[Header("Movement Settings :")]
	public MovementSettings XAxis;
	public MovementSettings YAxis;
	public MovementSettings ZAxis;

	// Internal Settings
	Light myLight;
	Transform myTransform;
	bool xStart, xEnd, xPosReset;
	float xPos, xMin, xMax;
	bool yStart, yEnd, yPosReset;
	float yPos, yMin, yMax;
	bool zStart, zEnd, zPosReset;
	float zPos, zMin, zMax;
	bool lightStart, lightEnd;
	float lightMin, lightMax;
	float currentSpeed;
	bool isRunning;
	float targetIntensity;
	float timePassed;
	float actionTime;

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
		xPosReset = true;
		xMin = XAxis.MinimumOffset;
		xMax = XAxis.MaximumOffset;
		yPos = myTransform.position.y;
		yPosReset = true;
		yMin = YAxis.MinimumOffset;
		yMax = YAxis.MaximumOffset;
		zPos = myTransform.position.z;
		xPosReset = true;
		zMin = ZAxis.MinimumOffset;
		zMax = ZAxis.MaximumOffset;
		currentSpeed = Speed / 10;
	}

	void FixedUpdate()
	{
		switch (LightType) {
		case LightSelection.Flickering:
			Flickering ();
			break;
		case LightSelection.Fluorecent:
			if (!isRunning) StartCoroutine(Fluorecent());
			break;
		}

		LightMovement ();
	}

	void LightMovement()
	{
		if (XAxis.EnableMovement) {
			if (xPosReset)
			{
				xPos = myTransform.position.x;
				xPosReset = false;
			}

			if (xMax > XAxis.MaximumOffset)
				xMax = XAxis.MaximumOffset;

			if (xMin < XAxis.MinimumOffset)
				xMin = XAxis.MinimumOffset;

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
		else
		{
			myTransform.position = new Vector3(xPos, myTransform.position.y, myTransform.position.z);
			xPosReset = true;
		}

		if (YAxis.EnableMovement) {
			if (yPosReset) {
				yPos = myTransform.position.y;
				yPosReset = false;
			}

			if (yMax > YAxis.MaximumOffset)
				yMax = YAxis.MaximumOffset;
			
			if (yMin < YAxis.MinimumOffset)
				yMin = YAxis.MinimumOffset;

			if (transform.position.y >= yMax) {
				yEnd = true;

				if (YAxis.Randomness > 0.0f && yStart) {
					if (Random.Range (0.0f, 1.1f) <= YAxis.Randomness)
						yMax = Random.Range (yPos, YAxis.MaximumOffset);
				} else if (YAxis.Randomness == 0.0f)
					yMax = YAxis.MaximumOffset;
				
				yStart = false;
			} else if (transform.position.y <= yMin) {
				yEnd = false;

				if (YAxis.Randomness > 0.0f && !yStart) {
					if (Random.Range (0.0f, 1.1f) <= YAxis.Randomness)
						yMin = Random.Range (yPos, YAxis.MinimumOffset);
				} else if (YAxis.Randomness == 0.0f)
					yMin = YAxis.MinimumOffset;
				
				yStart = true;
			}
			
			if (myTransform.position.y < yMax && !yEnd)
				myTransform.position = new Vector3 (myTransform.position.x, transform.position.y + YAxis.Speed, myTransform.position.z);
			else
				myTransform.position = new Vector3 (transform.position.x, myTransform.position.y - YAxis.Speed, myTransform.position.z);

			myTransform.position = new Vector3 (
				myTransform.position.x,
				Mathf.Clamp (myTransform.position.y, YAxis.MinimumOffset, YAxis.MaximumOffset),
				myTransform.position.z
			);
		} else {
			myTransform.position = new Vector3(myTransform.position.x, yPos, myTransform.position.z);
			yPosReset = true;
		}

		if (ZAxis.EnableMovement) {
			if (zPosReset) {
				zPos = myTransform.position.z;
				zPosReset = false;
			}

			if (zMax > ZAxis.MaximumOffset)
				zMax = ZAxis.MaximumOffset;
			
			if (zMin < ZAxis.MinimumOffset)
				zMin = ZAxis.MinimumOffset;

			if (transform.position.z >= zMax) {
				zEnd = true;

				if (ZAxis.Randomness > 0.0f && zStart) {
					if (Random.Range (0.0f, 1.1f) <= ZAxis.Randomness)
						zMax = Random.Range (zPos, ZAxis.MaximumOffset);
				} else if (ZAxis.Randomness == 0.0f)
					zMax = ZAxis.MaximumOffset;
				
				zStart = false;
			} else if (transform.position.z <= zMin) {
				zEnd = false;

				if (ZAxis.Randomness > 0.0f && !zStart) {
					if (Random.Range (0.0f, 1.1f) <= ZAxis.Randomness)
						zMin = Random.Range (zPos, ZAxis.MinimumOffset);
				} else if (ZAxis.Randomness == 0.0f)
					zMin = ZAxis.MinimumOffset;

				zStart = true;
			}
			
			if (myTransform.position.z < zMax && !zEnd)
				myTransform.position = new Vector3 (transform.position.x, myTransform.position.y, myTransform.position.z + ZAxis.Speed);
			else
				myTransform.position = new Vector3 (transform.position.x, myTransform.position.y, myTransform.position.z - ZAxis.Speed);

			myTransform.position = new Vector3 (
				myTransform.position.x,
				myTransform.position.y,
				Mathf.Clamp (myTransform.position.z, ZAxis.MinimumOffset, ZAxis.MaximumOffset)
			);
		} else {
			myTransform.position = new Vector3 (myTransform.position.x, myTransform.position.y, zPos);
			zPosReset = true;
		}
	}

	void Flickering()
	{
		if (Speed > 0) {
			float intensity = myLight.intensity;

			if (lightMax > MaximumIntensity)
				lightMax = MaximumIntensity;

			if (lightMin < MinimumIntensity)
				lightMin = MinimumIntensity;

			if (intensity >= lightMax) {
				lightEnd = true;

				if (FlickeringRandomness > 0.0f && lightStart) {
					if (Random.Range(0.0f, 1.1f) <= FlickeringRandomness)
							lightMax = Random.Range((MinimumIntensity +  MaximumIntensity) / 2, MaximumIntensity);
				}
				else if (FlickeringRandomness == 0.0f)
					lightMax = MaximumIntensity;

				lightStart = false;
			}
			else if (intensity <= lightMin)
			{
				lightEnd = false;

				if (SpeedOffset > 0 && Random.Range (0.0f, 1.1f) < SpeedFluctuation)
					currentSpeed = Random.Range (Speed, SpeedOffset) / 10;
				else
					currentSpeed = Speed / 10;

				if (FlickeringRandomness > 0.0f && !lightStart) {
					if (Random.Range(0.0f, 1.1f) <= FlickeringRandomness)
						lightMin = Random.Range(MinimumIntensity, (MinimumIntensity + MaximumIntensity) / 2);
				}
				else if (FlickeringRandomness == 0.0f)
					lightMin = MinimumIntensity;

				lightStart = true;
			}

			if (intensity < lightMax && !lightEnd)
				intensity += currentSpeed;
			else
				intensity -= currentSpeed;

			myLight.intensity = intensity;
			Mathf.Clamp(myLight.intensity, MinimumIntensity, MaximumIntensity);
		}
	}

	IEnumerator Fluorecent()
	{
		isRunning = true;

		if (myLight.intensity == MaximumIntensity) {
			if (FlickeringRandomness == 0)
				myLight.intensity = MinimumIntensity;
			else if (Random.Range (0.0f, 1.1f) < FlickeringRandomness)
				myLight.intensity = MinimumIntensity;
		}
		else
			myLight.intensity = MaximumIntensity;

		if (SpeedOffset > 0 && Random.Range(0.0f, 1.1f) < SpeedFluctuation) {
			float curSpeed = Random.Range (Speed, SpeedOffset);
			Debug.Log("hey");
			yield return new WaitForSeconds (0.5f / curSpeed);
			Debug.Log("yey!");
		}
		else
			yield return new WaitForSeconds (0.5f / Mathf.Abs(Speed));


		isRunning = false;
	}
}