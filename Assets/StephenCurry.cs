using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using weka.classifiers.trees;
using weka.classifiers.evaluation;
using weka.core;
using java.lang;
using java.util;
using weka.classifiers.functions;
using weka.classifiers;
using weka.core.converters;
using System.Threading;
using System.IO;
using System;
using Debug = UnityEngine.Debug;

public class StephenCurry : MonoBehaviour
{
    [SerializeField]
    private weka.classifiers.trees.M5P predictXForce, predictYForce, predictDistance;
    [SerializeField]
    private weka.core.Instances cases;
    [SerializeField]
    private GameObject ball, ballInstance, targetPoint;
    [SerializeField]
    private float maxValueFx, maxValueFy, minValueFx, minValueFy, step, bestFx, bestFy, targetDistance, shoot;
    [SerializeField]
    private Vector3 maxPosition, minPosition;

    private bool Testing_Running;

    public bool training, testing;
    public float shotsTried, shotsMade;
    public BallLogic holdedBall;
    public bool hasScored;
    
    // Start is called before the first frame update
    void Start()
    {
        shotsTried = 0;
        shotsMade = 0;
        StartCoroutine(Learning());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (testing && !Testing_Running) StartCoroutine(Testing());
    }

    public IEnumerator Shoot()
    {
        Vector3 fuerza = transform.up * bestFy + transform.forward * bestFx;

        if (!((bestFx == 0) && (bestFy == 0)))
        {
            hasScored = false;
            shotsTried++;
            ballInstance = holdedBall?.gameObject ?? Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
            if (testing)
            {
                holdedBall = ballInstance.GetComponent<BallLogic>();
            }
            Rigidbody rigidbody = ballInstance.GetComponent<Rigidbody>();
            holdedBall.prueba = this;
            holdedBall.Thrown();
            rigidbody.AddForce(fuerza, ForceMode.Impulse);

            if (testing)
            {
                holdedBall = null;
            }

            shoot++;
            yield return new WaitUntil(() => (hasScored || (rigidbody.transform.position.y < targetPoint.transform.position.y - 1) && rigidbody.velocity.y < 0));
            if (hasScored)
            {
                shotsMade++;
                hasScored = false;
                //Instance learningCase = new Instance(cases.numAttributes());
                //learningCase.setDataset(cases);
                //learningCase.setValue(0, bestFx);
                //learningCase.setValue(1, bestFy);
                //learningCase.setValue(2, targetDistance);
                //cases.add(learningCase);
                //using (StreamWriter w = System.IO.File.AppendText("Assets/Experiences.arff"))
                //{
                //    w.WriteLine(learningCase);
                //}
            }
            Destroy(ballInstance, 1);
        }
    }

    #region Learning Script

    IEnumerator Learning()
    {
        Time.timeScale = 5;
        string file = training ? "Assets/ShowMustGoOn.arff" : "Assets/Experiences.arff";
        cases = new weka.core.Instances(new java.io.FileReader(file));

        while (training)
        {
            float fx, fy, px = 0, pz = 0;
            fx = UnityEngine.Random.Range(minValueFx, maxValueFx);
            fy = UnityEngine.Random.Range(minValueFy, maxValueFy);
            GetRandomPosition(ref px, ref pz);
            transform.position = new Vector3(px, 0, pz);
            transform.LookAt(new Vector3(targetPoint.transform.position.x, 0, targetPoint.transform.position.z));
            ballInstance = Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
            ballInstance.GetComponent<BallLogic>().prueba = this;
            ballInstance.GetComponent<BallLogic>().Thrown();
            Rigidbody rb = ballInstance.GetComponent<Rigidbody>();
            rb.AddForce(transform.up * fy + transform.forward * fx, ForceMode.Impulse);
            Destroy(ballInstance, 10);
            yield return new WaitUntil(() => (hasScored || (rb.transform.position.y < targetPoint.transform.position.y - 1) && rb.velocity.y < 0));

            if (hasScored)
            {
                print("ENTRENAMIENTO: con fuerza Fx " + fx + " y Fy=" + fy + " se alcanzó una distancia de " + Vector3.Distance(rb.transform.position, transform.position) + " m");
                Instance learningCase = new Instance(cases.numAttributes());
                learningCase.setDataset(cases);
                learningCase.setValue(0, fx);
                learningCase.setValue(1, fy);
                learningCase.setValue(2, Vector3.Distance(rb.transform.position, transform.position));
                cases.add(learningCase);
                using (StreamWriter w = System.IO.File.AppendText(file))
                {
                    w.WriteLine(learningCase);
                }
            }
            rb.isKinematic = true; rb.GetComponent<SphereCollider>().isTrigger = true;
            Destroy(ballInstance, 1);
        }
        GenerateSystem();
    }

    #endregion

    private void GenerateSystem()
    {
        predictXForce = new M5P();
        cases.setClassIndex(0);
        predictXForce.buildClassifier(cases);

        predictYForce = new M5P();
        cases.setClassIndex(1);
        predictYForce.buildClassifier(cases);

        Time.timeScale = 1;
    }
    
    public void GetRandomPosition(ref float px, ref float pz)
    {
        px = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
        pz = UnityEngine.Random.Range(minPosition.z, maxPosition.z);
    }

    #region Testing AI
    IEnumerator Testing()
    {
        Testing_Running = true;
        float px = 0, pz = 0;
        GetRandomPosition(ref px, ref pz);
        transform.position = new Vector3(px, 0, pz);
        yield return CalculateForces();
        yield return Shoot();
        Debug.Log($"Porcentaje currente: {shotsMade}/{shotsTried} - {(float)(shotsMade / shotsTried * 100)}");
        Testing_Running = false;
    }
    #endregion

    public IEnumerator CalculateForces()
    {
        transform.LookAt(new Vector3(targetPoint.transform.position.x, 0, targetPoint.transform.position.z));
        targetDistance = Vector3.Distance(targetPoint.transform.position, transform.position + new Vector3(-0.1f, 1, 0));

        Instance testCase = new Instance(cases.numAttributes());
        testCase.setDataset(cases);
        testCase.setValue(2, targetDistance);

        bestFx = (float)predictXForce.classifyInstance(testCase);

        Instance testCase2 = new Instance(cases.numAttributes());
        testCase2.setDataset(cases);
        testCase2.setValue(0, bestFx);
        testCase2.setValue(2, targetDistance);

        bestFy = (float)predictYForce.classifyInstance(testCase2);

        yield return null;
    }
}
