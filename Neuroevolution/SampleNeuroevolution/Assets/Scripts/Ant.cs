using UnityEngine;

public class Ant : MonoBehaviour {

	public Vector2 Position => transform.position;
	public Vector2 Target => ClosestSeed().transform.position;
	public Vector2 Velocity => GetComponent<Rigidbody2D>().velocity;

	public double ConsumeDistance = 0.25f;
	public float MaxSpeed = 10f;
	public float WanderJitter { get; } = 0.5f;
	public float WanderDistance { get; } = 2f;
	public float WanderRadius { get; } = 5f;

	private SteeringBehaviour _steeringBehavior;

	private void Start()
	{
		_steeringBehavior = new SteeringBehaviour();
	}

	// Update is called once per frame
	void Update ()
	{
		GetComponent<Rigidbody2D>().AddForce(_steeringBehavior.Seek(this));
		AdjustRotation();
		ConsumeSeed();
	}

	private void ConsumeSeed()
	{
		var closestSeed = ClosestSeed();
		if (Vector2.Distance(ClosestSeed().position, transform.position) < ConsumeDistance)
		{
			Destroy(closestSeed.gameObject);
		}
	}


	private Transform ClosestSeed()
	{
		var closestDistance = float.PositiveInfinity;
		GameObject closestSeed = null;
		
		foreach (var seed in GameManager.Instance.Seeds)
		{
			if (seed == null) continue;
			
			var dist = Vector2.Distance(seed.transform.position, transform.position);
			if (dist < closestDistance) 
			{
				closestDistance = dist;
				closestSeed = seed;
			}
		}

		return closestSeed == null ? null : closestSeed.transform;
	}
	
	private void AdjustRotation()
	{
		var angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg + 270f;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}