/* ----------------------------------------------
 * Version 3.2
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
	[Header("Script Version : 3.2")]
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

		xPosReset = true;
		yPosReset = true;
		xPosReset = true;
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
		if (XAxis.EnableMovement && XAxis.Speed > 0) {
			if (xPosReset)
			{
				xPos = myTransform.position.x;
				xPosReset = false;
			}

			float curMin = xPos + XAxis.MinimumOffset;
			float curMax = xPos + XAxis.MaximumOffset;

			if (xMax > curMax)
				xMax = curMax;

			if (xMin < curMin)
				xMin = curMin;

			if (transform.position.x >= xMax)
			{
				xEnd = true;

				if (xStart && XAxis.Randomness > 0.0f)
				{
					if (Random.Range(0.0f, 1.1f) <= XAxis.Randomness)
						xMax = xPos + Random.Range(xPos, XAxis.MaximumOffset);
				}
				else if (XAxis.Randomness == 0.0f)
					xMax = curMax;

				xStart = false;
			}
			else if (transform.position.x <= xMin)
			{
				xEnd = false;

				if (XAxis.Randomness > 0.0f && !xStart)
				{
					if (Random.Range(0.0f, 1.1f) <= XAxis.Randomness)
						xMin = xPos + Random.Range(xPos, XAxis.MinimumOffset);
				}
				else if (XAxis.Randomness == 0.0f)
					xMin = curMin;

				xStart = true;
			}

			if (myTransform.position.x < xMax && !xEnd)
				myTransform.position = new Vector3(transform.position.x + XAxis.Speed, myTransform.position.y, myTransform.position.z);
			else
				myTransform.position = new Vector3(transform.position.x - XAxis.Speed, myTransform.position.y, myTransform.position.z);

			myTransform.position = new Vector3(
				Mathf.Clamp (myTransform.position.x, curMin, curMax),
				myTransform.position.y,
				myTransform.position.z
				);
		}
		else if (!xPosReset)
		{
			myTransform.position = new Vector3(xPos, myTransform.position.y, myTransform.position.z);
			xMin = xPos + XAxis.MinimumOffset;
			xMax = xPos + XAxis.MaximumOffset;
			xPosReset = true;
		}

		if (YAxis.EnableMovement && YAxis.Speed > 0) {
			if (yPosReset) {
				yPos = myTransform.position.y;
				yPosReset = false;
			}

			float curMin = yPos + YAxis.MinimumOffset;
			float curMax = yPos + YAxis.MaximumOffset;

			if (yMax > curMax)
				yMax = curMax;
			
			if (yMin < curMin)
				yMin = curMin;

			if (transform.position.y >= yMax) {
				yEnd = true;

				if (YAxis.Randomness > 0.0f && yStart) {
					if (Random.Range (0.0f, 1.1f) <= YAxis.Randomness)
						yMax = yPos + Random.Range (yPos, YAxis.MaximumOffset);
				} else if (YAxis.Randomness == 0.0f)
					yMax = curMax;
				
				yStart = false;
			} else if (transform.position.y <= yMin) {
				yEnd = false;

				if (YAxis.Randomness > 0.0f && !yStart) {
					if (Random.Range (0.0f, 1.1f) <= YAxis.Randomness)
						yMin = yPos + Random.Range (YAxis.MinimumOffset, yPos);
				} else if (YAxis.Randomness == 0.0f)
					yMin = curMin;
				
				yStart = true;
			}
			
			if (myTransform.position.y < yMax && !yEnd)
				myTransform.position = new Vector3 (myTransform.position.x, transform.position.y + YAxis.Speed, myTransform.position.z);
			else
				myTransform.position = new Vector3 (transform.position.x, myTransform.position.y - YAxis.Speed, myTransform.position.z);

			myTransform.position = new Vector3 (
				myTransform.position.x,
				Mathf.Clamp (myTransform.position.y, curMin, curMax),
				myTransform.position.z
			);
		} else if (!yPosReset){
			myTransform.position = new Vector3(myTransform.position.x, yPos, myTransform.position.z);
			yMax = yPos + YAxis.MaximumOffset;
			yMin = yPos + YAxis.MinimumOffset;
			yPosReset = true;
		}

		if (ZAxis.EnableMovement && ZAxis.Speed > 0) {
			if (zPosReset) {
				zPos = myTransform.position.z;
				zPosReset = false;
			}

			float curMax = zPos + ZAxis.MaximumOffset;
			float curMin = zPos + ZAxis.MinimumOffset;

			if (zMax > curMax)
				zMax = curMax;
			
			if (zMin < curMin)
				zMin = curMin;

			if (transform.position.z >= zMax) {
				zEnd = true;

				if (ZAxis.Randomness > 0.0f && zStart) {
					if (Random.Range (0.0f, 1.1f) <= ZAxis.Randomness)
						zMax = zPos + Random.Range (zPos, ZAxis.MaximumOffset);
				} else if (ZAxis.Randomness == 0.0f)
					zMax = curMax;
				
				zStart = false;
			} else if (transform.position.z <= zMin) {
				zEnd = false;

				if (ZAxis.Randomness > 0.0f && !zStart) {
					if (Random.Range (0.0f, 1.1f) <= ZAxis.Randomness)
						zMin = zPos + Random.Range (zPos, ZAxis.MinimumOffset);
				} else if (ZAxis.Randomness == 0.0f)
					zMin = curMin;

				zStart = true;
			}
			
			if (myTransform.position.z < zMax && !zEnd)
				myTransform.position = new Vector3 (transform.position.x, myTransform.position.y, myTransform.position.z + ZAxis.Speed);
			else
				myTransform.position = new Vector3 (transform.position.x, myTransform.position.y, myTransform.position.z - ZAxis.Speed);

			myTransform.position = new Vector3 (
				myTransform.position.x,
				myTransform.position.y,
				Mathf.Clamp (myTransform.position.z, curMin, curMax)
			);
		} else if (!zPosReset) {
			myTransform.position = new Vector3 (myTransform.position.x, myTransform.position.y, zPos);
			zMax = zPos + ZAxis.MaximumOffset;
			zMin = zPos + ZAxis.MinimumOffset;
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

		if (myLight.intensity == MinimumIntensity) {
			if (FlickeringRandomness == 0)
				myLight.intensity = MaximumIntensity;
			else if (Random.Range (0.0f, 1.1f) < FlickeringRandomness)
				myLight.intensity = Random.Range(MinimumIntensity, MaximumIntensity);
		}
		else
			myLight.intensity = MinimumIntensity;

		if (SpeedOffset > 0 && Random.Range(0.0f, 1.1f) < SpeedFluctuation) {
			float curSpeed = Random.Range (Speed, SpeedOffset);
			yield return new WaitForSeconds (0.5f / curSpeed);
		}
		else
			yield return new WaitForSeconds (0.5f / Mathf.Abs(Speed));


		isRunning = false;
	}
}