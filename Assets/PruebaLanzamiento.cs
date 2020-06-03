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

public class PruebaLanzamiento : MonoBehaviour
{
    [SerializeField]
    private weka.classifiers.trees.M5P predictXForce, predictYForce, predictDistance;
    [SerializeField]
    private weka.core.Instances cases;
    [SerializeField]
    private GameObject ball, ballInstance, targetPoint, canasta;
    [SerializeField]
    private float maxValueFx, maxValueFy, minValueFx, minValueFy, step, bestFx, bestFy, targetDistance, shoot;
    [SerializeField]
    private bool yah = false, training;
    [SerializeField]
    private Vector3 maxPosition, minPosition;
    public bool hasScored;

    // Start is called before the first frame update
    void Start()
    {
        hasScored = false;
        StartCoroutine(Learning());
    }

    IEnumerator Learning()
    {
        Time.timeScale = 5;
        cases = new weka.core.Instances(new java.io.FileReader("Assets/Experiences.arff"));

        while (training)
        {
            float fx, fy, px = 0, pz = 0;
            fx = UnityEngine.Random.Range(minValueFx, maxValueFx);
            fy = UnityEngine.Random.Range(minValueFy, maxValueFy);
            GetRandomPosition(ref px, ref pz);
            transform.position = new Vector3(px, 0, pz);
            transform.LookAt(canasta.transform);
            ballInstance = Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
            ballInstance.GetComponent<Ball>().prueba = this;
            Destroy(ballInstance, 30);
            Rigidbody rb = ballInstance.GetComponent<Rigidbody>();
            rb.AddForce(transform.up * fy + transform.forward * fx, ForceMode.Impulse);
            yield return new WaitUntil(() => (ballInstance.GetComponent<Ball>().hasScored || (rb.transform.position.y < targetPoint.transform.position.y - 1) && rb.velocity.y < 0));

            if (ballInstance.GetComponent<Ball>().hasScored)
            {
                print("ENTRENAMIENTO: con fuerza Fx " + fx + " y Fy=" + fy + " se alcanzó una distancia de " + Vector3.Distance(rb.transform.position, transform.position) + " m");
                Instance learningCase = new Instance(cases.numAttributes());
                learningCase.setDataset(cases);
                learningCase.setValue(0, fx);
                learningCase.setValue(1, fy);
                learningCase.setValue(2, Vector3.Distance(rb.transform.position, transform.position));
                cases.add(learningCase);
                using (StreamWriter w = System.IO.File.AppendText("Assets/Experiences.arff"))
                {
                    w.WriteLine(learningCase);
                }
            }
            rb.isKinematic = true; rb.GetComponent<Collider>().isTrigger = true;
            Destroy(ballInstance, 1);
            hasScored = false;
        }

        predictXForce = new M5P();
        cases.setClassIndex(0);
        predictXForce.buildClassifier(cases);

        predictYForce = new M5P();
        cases.setClassIndex(1);
        predictYForce.buildClassifier(cases);

        hasScored = false;
        yah = true;
        Time.timeScale = 1;

        if (yah && shoot < 1)
        {
            transform.LookAt(canasta.transform);
            targetDistance = Vector3.Distance(targetPoint.transform.position, transform.position + new Vector3(-0.1f, 1, 0));// + new Vector3(-0.1f, 1, 0));

            Instance testCase = new Instance(cases.numAttributes());
            testCase.setDataset(cases);
            testCase.setValue(2, targetDistance);

            bestFx = (float)predictXForce.classifyInstance(testCase);

            Instance testCase2 = new Instance(cases.numAttributes());
            testCase2.setDataset(cases);
            testCase2.setValue(0, bestFx);
            testCase2.setValue(2, targetDistance);

            bestFy = (float)predictYForce.classifyInstance(testCase2);

            Vector3 fuerza = transform.up * bestFy + transform.forward * bestFx;

            Debug.Log($"Fuerza lanzada: {fuerza}");

            if (!((bestFx == 0) && (bestFy == 0)))
            {
                ballInstance = Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
                Rigidbody rigidbody = ballInstance.GetComponent<Rigidbody>();
                rigidbody.AddForce(fuerza, ForceMode.Impulse);
                shoot++;
                yield return new WaitUntil(() => (ballInstance.GetComponent<Ball>().hasScored || (rigidbody.transform.position.y < targetPoint.transform.position.y - 1) && rigidbody.velocity.y < 0));
                if (ballInstance.GetComponent<Ball>().hasScored)
                {
                    print("Ya he tirado, Fx: " + bestFx + ", Fy: " + bestFy);
                    Instance learningCase = new Instance(cases.numAttributes());
                    learningCase.setDataset(cases);
                    learningCase.setValue(0, bestFx);
                    learningCase.setValue(1, bestFy);
                    learningCase.setValue(2, targetDistance);
                    cases.add(learningCase);
                    using (StreamWriter w = System.IO.File.AppendText("Assets/Experiences.arff"))
                    {
                        w.WriteLine(learningCase);
                    }
                }
            }
        }
    }

    private void GetRandomPosition(ref float px, ref float pz)
    {
        px = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
        pz = UnityEngine.Random.Range(minPosition.z, maxPosition.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
