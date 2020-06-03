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
            rb.AddForce(new Vector3(0, fy, 0) + transform.forward * fx, ForceMode.Impulse);
            yield return new WaitUntil(() => (ballInstance.GetComponent<Ball>().hasScored || (rb.transform.position.y < targetPoint.transform.position.y) && rb.velocity.y < 0));

            if (ballInstance.GetComponent<Ball>().hasScored)
            {
                print("ENTRENAMIENTO: con fuerza Fx " + fx + " y Fy=" + fy + " se alcanzó una distancia de " + Vector3.Distance(rb.transform.position, transform.position) + " m");
                Instance casoAaprender = new Instance(cases.numAttributes());
                casoAaprender.setDataset(cases);
                casoAaprender.setValue(0, fx);
                casoAaprender.setValue(1, fy);
                casoAaprender.setValue(2, Vector3.Distance(rb.transform.position, transform.position));
                cases.add(casoAaprender);
                using (StreamWriter w = System.IO.File.AppendText("Assets/Experiences.arff"))
                {
                    w.WriteLine(casoAaprender);
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

        yah = true;
        Time.timeScale = 1;
    }

    private void GetRandomPosition(ref float px, ref float pz)
    {
        px = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
        pz = UnityEngine.Random.Range(minPosition.z, maxPosition.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (yah && shoot < 1)
        {
            hasScored = false;
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

            if (!((bestFx == 0) && (bestFy == 0)))
            {
                /*if(hasScored) {
                    Instance learningCase = new Instance(cases.numAttributes());
                    learningCase.setDataset(cases);
                    learningCase.setValue(0, bestFx);
                    learningCase.setValue(1, bestFy);
                    learningCase.setValue(2, targetDistance);
                    cases.add(learningCase);
                    File outputFile = new File("Assets/Experiences.arff");
                    if(!outputFile.exists()) System.IO.File.Create(outputFile.getAbsoluteFile().toString()).Dispose();
                    ArffSaver saver = new ArffSaver();
                    saver.setInstances(cases);
                    saver.setDestination(outputFile);
                    saver.setFile(outputFile);
                    saver.writeBatch();
                }*/
                ballInstance = Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
                Rigidbody rigidbody = ballInstance.GetComponent<Rigidbody>();
                rigidbody.AddForce(new Vector3(0, bestFy, 0) + transform.forward * bestFx, ForceMode.Impulse);
                shoot++;
                print("Ya he tirado, Fx: " + bestFx + ", Fy: " + bestFy);
            }
        }
    }
}
