using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using weka.classifiers.trees;
using weka.classifiers.evaluation;
using weka.core;
using java.io;
using java.lang;
using java.util;
using weka.classifiers.functions;
using weka.classifiers;
using weka.core.converters;

public class PruebaLanzamiento : MonoBehaviour
{
    [SerializeField]
    private weka.classifiers.trees.M5P predictXForce, predictDistance;
    [SerializeField]
    private weka.core.Instances cases;
    [SerializeField]
    private GameObject ball, ballInstance, targetPoint;
    [SerializeField]
    private float maxValueFx, maxValueFy, step, bestFx, bestFy, targetDistance;
    [SerializeField]
    private Rigidbody rigidbody;
    [SerializeField]
    private bool yah = false, yeh = false;
    [SerializeField]
    private int shoot;

    // Start is called before the first frame update
    void Start(){
        shoot = 0;
        StartCoroutine(Learning());
    }

    IEnumerator Learning() {
        cases = new weka.core.Instances(new java.io.FileReader("Assets/Experiences.arff"));

        predictXForce = new M5P();
        cases.setClassIndex(0);
        predictXForce.buildClassifier(cases);

        predictDistance = new M5P();
        cases.setClassIndex(2);
        predictDistance.buildClassifier(cases);

        targetDistance = Vector3.Distance(targetPoint.transform.position, transform.position + new Vector3(0, 1, 0));
        yah = true;
       
        yield return null;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(yah && shoot < 1) {
            float minorDistance = 1e9f;
            for(float Fy = 1; Fy < maxValueFy; Fy = Fy + step) {
                Instance testCase = new Instance(cases.numAttributes());
                testCase.setDataset(cases);
                testCase.setValue(1, Fy);
                testCase.setValue(2, targetDistance);
                float Fx = (float) predictXForce.classifyInstance(testCase);
                if((Fx >= 1) && (Fx <= maxValueFx)) {
                    Instance testCase2 = new Instance(cases.numAttributes());
                    testCase2.setDataset(cases);
                    testCase2.setValue(0, Fx);
                    testCase2.setValue(1, Fy);
                    float distancePrediction = (float) predictDistance.classifyInstance(testCase2);
                    if(Mathf.Abs(distancePrediction - targetDistance) < minorDistance) {
                        minorDistance = Mathf.Abs(distancePrediction - targetDistance);
                        bestFx = Fx;
                        bestFy = Fy;
                    }
                }
            }

            if(!((bestFx == 0) && (bestFy == 0))) {
                Instance learningCase = new Instance(cases.numAttributes());
                learningCase.setDataset(cases);
                learningCase.setValue(0, bestFx);
                learningCase.setValue(1, bestFy);
                learningCase.setValue(2, targetDistance);
                cases.add(learningCase);
                File outputFile = new File("Assets/FinalExperiences.arff");
                if(!outputFile.exists()) System.IO.File.Create(outputFile.getAbsoluteFile().toString()).Dispose();
                ArffSaver saver = new ArffSaver();
                saver.setInstances(cases);
                saver.setDestination(outputFile);
                saver.setFile(outputFile);
                saver.writeBatch();
                ballInstance = Instantiate(ball, transform.position + new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
                rigidbody = ballInstance.GetComponent<Rigidbody>();
                rigidbody.AddForce(new Vector3(-bestFx, bestFy, 0), ForceMode.Impulse);
                yah = false;
                shoot++;
            }
        }
    }
}
