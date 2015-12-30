using UnityEngine;
using System.Collections;

using Klax.Common;

namespace Klax.World
{
	public class Block : MonoBehaviour
	{
		const float straightAngle = 90.0f;
		const float angleSpeed = 15.0f;

		public BlockColor Color;
		public Renderer Renderer;
		public Collider Collider;
		public Rigidbody Rigidbody;
		public GameObject GameObject;
		public Transform Transform;

		bool droped = false;
		Vector3 axis = - Vector3.right;

		public void StartMoving()
		{
			droped = false;
			StartCoroutine(Move());
		}

		IEnumerator Move()
		{
			while (!droped)
			{
				yield return StartCoroutine(Step0());
				yield return StartCoroutine(Step1());
			}
			Collider.enabled = true;
			Rigidbody.useGravity = true;
		}

		IEnumerator Step0()
		{
			var center = Transform.localPosition;
			var offset = Transform.localScale / 2;
			var point = center - offset;

			float time = 0;
			float timeLimit = straightAngle / angleSpeed;

			while (time < timeLimit)
			{
				time += Time.deltaTime;
				var delta = Time.deltaTime * angleSpeed;
				Transform.RotateAround(point, axis, delta);
				yield return new WaitForEndOfFrame();
			}
			
			Transform.eulerAngles = new Vector3(-straightAngle, 0, 0);
			center = new Vector3(center.x, point.y + offset.z, point.z - offset.y);
			Transform.localPosition = center;

			yield break;
		}

		IEnumerator Step1()
		{
			var center = Transform.localPosition;
			var offset = Transform.localScale / 2;
			var offset2 = new Vector3(0, offset.z, offset.y);
			var point = center - offset2;

			var time = 0.0;
			float timeLimit = straightAngle / angleSpeed;

			while (time < timeLimit)
			{
				time += Time.deltaTime;
				Transform.RotateAround(point, axis, Time.deltaTime * angleSpeed);
				yield return new WaitForEndOfFrame();
			}
			
			Transform.localEulerAngles = Vector3.zero;
			center = new Vector3(center.x, point.y + offset.y, point.z - offset.z);
			Transform.localPosition = center;
		}

		void OnDisable()
		{
			Collider.enabled = false;
			Rigidbody.useGravity = false;
			droped = false;
		}

		void OnTriggerEnter()
		{
			droped = true;
		}
	}
}