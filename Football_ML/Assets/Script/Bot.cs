using MLAgents;
using MLAgents.Sensors;
using UnityEngine;

public class Bot : Agent
{
    [Header("Speed"), Range(1, 50)]
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private Transform goalNet;

    private Rigidbody rb;
    private Rigidbody ball_rb;

    private bool touchTheBall, touchTheBallRewared;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball_rb = GameObject.FindGameObjectWithTag("football").GetComponent<Rigidbody>();

        touchTheBall = touchTheBallRewared = false;
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = ball_rb.velocity = Vector3.zero;
        rb.angularVelocity = ball_rb.angularVelocity = Vector3.zero;

        Vector3 positionBot = new Vector3(Random.Range(-1f, 1f), 0.1f, Random.Range(-2f, 1f));
        transform.position = positionBot;

        Vector3 positionFootball = new Vector3(Random.Range(-0.5f, 0.5f), 0.1f, Random.Range(-1f, 0.5f));
        ball_rb.position = positionFootball;

        Football.goal = false;
        touchTheBall = false;
        touchTheBallRewared = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(ball_rb.position);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Action
        Vector3 control = Vector3.zero;
        control.x = vectorAction[0];
        control.z = vectorAction[1];
        rb.AddForce(control * speed);

        //Rewards
        if (touchTheBall && !touchTheBallRewared)
        {
            SetReward(0.5f);
            touchTheBallRewared = true;
        }
        if (Vector3.Distance(ball_rb.transform.position, goalNet.position) < 1f)
        {
            SetReward(0.1f);
        }
        if (Football.goal)
        {
            SetReward(1);
            EndEpisode();
        }

        //Out of range
        if(transform.position.y < 0 || ball_rb.position.y < 0 || ball_rb.position.x > 1 || ball_rb.position.x < -1)
        {
            SetReward(-1);
            EndEpisode();
        }

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("football") && touchTheBall != true)
        {
            touchTheBall = true;
        }
    }
}
