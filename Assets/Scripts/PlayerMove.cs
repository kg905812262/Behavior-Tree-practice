using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	public float speed = 20;

	private Rigidbody rd;
	private AudioSource footStepsAudio;
	private float h;
	private float v;

	private void Awake()
	{
		rd = GetComponent<Rigidbody>();
		footStepsAudio = GetComponent<AudioSource>();
	}

	private void Start()
	{
		h = 0f;
		v = 0f;
	}

	private void Update()
	{
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		FootStepsSFX();
	}

	void FixedUpdate () {
		Vector3 move = v * transform.forward + h * transform.right;
		rd.MovePosition(transform.position + move * speed * Time.deltaTime);
		Quaternion turnRotation = Quaternion.Euler(0f, (Input.GetAxis("Mouse X")) * 5, 0f);
		rd.MoveRotation(rd.rotation * turnRotation);
		//transform.Rotate(Vector3.up * (Input.GetAxis("Mouse X")) * 5, Space.World);
	}

	private void FootStepsSFX()
	{
		if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
		{
			footStepsAudio.Stop();
		}
		else if (!footStepsAudio.isPlaying)
		{
			footStepsAudio.Play();
		}

		if (footStepsAudio.time > 0.35f)
		{
			footStepsAudio.time = 0f;
		}
	}
}
