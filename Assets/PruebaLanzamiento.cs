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
using System.Threading;

public class PruebaLanzamiento : MonoBehaviour
{
    [SerializeField]
    private weka.classifiers.trees.M5P predictXForce, predictDistance;
    [SerializeField]
    private weka.core.Instances cases;
    [SerializeField]
    private GameObject ball, ballInstance, targetPoint, canasta;
    [SerializeField]
    private float maxValueFx, maxValueFy, minValueFx, minValueFy, step, bestFx, bestFy, targetDistance, shoot;
    [SerializeField]
    private bool yah = false;
    public bool hasScored;

    // Start is called before the first frame update
    void Start(){
        hasScored = false;
        StartCoroutine(Learning());
    }

    IEnumerator Learning() {
        Time.timeScale = 7;
        cases = new weka.core.Instances(new java.io.FileReader("Assets/Experiences.arff"));

        for(float Fx = minValueFx; Fx <= maxValueFx; Fx = Fx + step)                      //Bucle de planificación de la fuerza FX durante el entrenamiento
            {
            for(float Fy = minValueFy; Fy <= maxValueFy; Fy = Fy + step)                    //Bucle de planificación de la fuerza FY durante el entrenamiento
            {
                ballInstance = Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
                Rigidbody rb = ballInstance.GetComponent<Rigidbody>();              //Crea una pelota física
                rb.AddForce(new Vector3(0, Fy, 0) + transform.forward * Fx, ForceMode.Impulse);                //y la lanza con las fuerza Fx y Fy
                yield return new WaitUntil(() => ((rb.transform.position.y < targetPoint.transform.position.y) && rb.velocity.y < 0));       //... y espera a que la pelota llegue al suelo

                if(hasScored) {
                    print("ENTRENAMIENTO: con fuerza Fx " + Fx + " y Fy=" + Fy + " se alcanzó una distancia de " + Vector3.Distance(rb.transform.position, transform.position) + " m");
                    Instance casoAaprender = new Instance(cases.numAttributes());
                    casoAaprender.setDataset(cases);                          //crea un registro de experiencia
                    casoAaprender.setValue(0, Fx);                                         //guarda los datos de las fuerzas Fx y Fy utilizadas
                    casoAaprender.setValue(1, Fy);
                    casoAaprender.setValue(2, Vector3.Distance(rb.transform.position, transform.position));
                    cases.add(casoAaprender);                                 //guarda el registro en la lista casosEntrenamiento
                }

                rb.isKinematic = true; rb.GetComponent<Collider>().isTrigger = true;   //...opcional: paraliza la pelota
                Destroy(ballInstance, 1);                                              //...opcional: destruye la pelota
                hasScored = false;
            }                                                                          //FIN bucle de lanzamientos con diferentes de fuerzas
        }


        File salida = new File("Assets/Experiences.arff");
        if(!salida.exists())
            System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
        ArffSaver saver = new ArffSaver();
        saver.setInstances(cases);
        saver.setFile(salida);
        saver.writeBatch();

        predictXForce = new M5P();
        cases.setClassIndex(0);
        predictXForce.buildClassifier(cases);

        predictDistance = new M5P();
        cases.setClassIndex(2);
        predictDistance.buildClassifier(cases);

        yah = true;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(yah && shoot < 5) {
            hasScored = false;
            transform.LookAt(canasta.transform);
            targetDistance = Vector3.Distance(targetPoint.transform.position, transform.position + new Vector3(-0.1f, 1, 0));
            float minorDistance = 1e9f;
            for(float Fy = 10; Fy < maxValueFy; Fy = Fy + step) {
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
                    if(Mathf.Abs(distancePrediction) < minorDistance) {
                        minorDistance = Mathf.Abs(distancePrediction - targetDistance);
                        bestFx = Fx;
                        bestFy = Fy;
                    }
                }
            }

            if(!((bestFx == 0) && (bestFy == 0))) {
                if(hasScored) {
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
                }
                ballInstance = Instantiate(ball, transform.position + new Vector3(-0.1f, 1, 0), transform.rotation) as GameObject;
                Rigidbody rigidbody = ballInstance.GetComponent<Rigidbody>();
                rigidbody.AddForce(new Vector3(0, bestFy, 0) + transform.forward * bestFx, ForceMode.Impulse);
                shoot++;
                print("Ya he tirado, Fx: " + bestFx + ", Fy: " + bestFy);
            }
        }
    }
}
