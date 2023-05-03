using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgentsExamples;


public class Predictor : MonoBehaviour
{
    private Agent agent;

    private Transform cube;
    public Transform target;

    private Transform hip;
    public GameObject walker;

    // private int completedSteps;
    private Transform footL, footR;

    public Vector3 forward;
    public Vector3 position;
    public Vector3 last_pos;
    public bool isLeft = false;

    const float rewardScale = 1.0f;
    const float groundHeight = 0.5277205f;

    const float maxRadian = Mathf.PI / 9.0f;
    const float stepSize = 1.4f;
    const float stepWidth = 0.4f;



    private void DrawPath()
    {
        Vector3 start = new Vector3(hip.position.x, 0.0f, hip.position.z);
        Vector3 forward = Vector3.Normalize(new Vector3(hip.forward.x, 0.0f, hip.forward.z));

        Vector3 end;
        int iter = 0;
        while(Vector3.Distance(start, target.position) > 1.5f)
        {
            iter++;
            forward = Vector3.RotateTowards(forward, target.position - start, maxRadian, 0.0f);
            end = start + forward * stepSize;
            
            Debug.DrawLine(start, end, Color.red, 10.0f);
            start = end;

            if (iter >= 10)
                break;
        }
    }


    public void Awake()
    {
        hip = walker.transform.GetChild(0);
        cube = walker.transform.GetChild(2);
        agent = walker.GetComponent<Agent>();

        footL = hip.Find("thighL/shinL/footL");
        footR = hip.Find("thighR/shinR/footR");
    }


    public void Start()
    {
        // Debug.Log(transform);
    }


    public void SetNextStep()
    {
        this.forward = Vector3.RotateTowards(this.forward, target.position - this.position, maxRadian, 0.0f);
        this.position += this.forward * stepSize;

        Vector3 left = Vector3.Cross(this.forward, Vector3.up);
        Vector3 spawnPos = this.position + (this.isLeft ? left : -left) * stepWidth;
        spawnPos.y = groundHeight;

        transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(this.forward, Vector3.up));
    }


    public void InitializeStep()
    {
        // completedSteps = 0;

        this.isLeft = (Random.value < 0.5f);

        this.position = new Vector3(hip.position.x, 0.0f, hip.position.z);
        this.forward = Vector3.Normalize(new Vector3(hip.forward.x, 0.0f, hip.forward.z));
        
        this.last_pos = this.isLeft ? footR.position : footL.position;

        SetNextStep();
    }


    public void FixedUpdate()
    {
        float reward = 0.0f;
        if (isLeft)
        {
            reward = rewardScale * (-Vector3.Distance(this.last_pos, footR.position)/stepSize);
        }
        else
        {
            reward = rewardScale * (-Vector3.Distance(this.last_pos, footL.position)/stepSize);
        }

        agent.AddReward(reward);
    }


    void OnCollisionEnter(Collision col)
    {
        if ((col.gameObject.name == "footR") || (col.gameObject.name == "footL"))
        {
            if(isLeft == (col.gameObject.name == "footL"))
            {
                // completedSteps += 1;
                agent.AddReward(5.0f);
                this.isLeft = (col.gameObject.name != "footL");
                this.last_pos = this.isLeft ? footR.position : footL.position;
                SetNextStep();
            }
            // DrawPath();
        }
    }
}
