using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    private int CuantasCamarasQuieresIyo;
    public List<Camera> PosLasCamarasxD;
    private int EnQueCamaraEstoy = 0;

    // Start is called before the first frame update
    void Start()
    {
        CuantasCamarasQuieresIyo = PosLasCamarasxD.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            PosLasCamarasxD[EnQueCamaraEstoy].enabled = false;
            EnQueCamaraEstoy++;
            if (EnQueCamaraEstoy == CuantasCamarasQuieresIyo) EnQueCamaraEstoy = 0;
            PosLasCamarasxD[EnQueCamaraEstoy].enabled = true;
        } else if (Input.GetKeyDown(KeyCode.E))
        {
            PosLasCamarasxD[EnQueCamaraEstoy].enabled = false;
            EnQueCamaraEstoy--;
            if (EnQueCamaraEstoy == -1) EnQueCamaraEstoy = CuantasCamarasQuieresIyo - 1;
            PosLasCamarasxD[EnQueCamaraEstoy].enabled = true;
        }
    }
}
