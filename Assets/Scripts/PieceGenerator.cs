using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    GameRules gr;
    public float maxAxis = 3.5f;
    public GameObject whitePrefab;
    public GameObject blackPrefab;
    bool waiting = false;
    public float maxWaitTime = 5f;
    public float minWaitTime = 5f;
    int pieceCount = 0;

    private void Start() {
        gr = GameRules.instance;
    }
    void Update()
    {
        if (!waiting) {
            generatePiece();
            StartCoroutine(wait());
        }
    }

    IEnumerator wait() {
        waiting = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        waiting = false;
    }

    void generatePiece() {
        pieceCount++;
        Vector3 position;
        do {
            float x,y;
            x = Random.Range(-maxAxis, maxAxis);
            y = Random.Range(-maxAxis, maxAxis);
            position = new Vector3(x, y, 0);
        } while (Physics.CheckSphere(position, gr.pieceRadius));

        GameObject _piece = (pieceCount % 2 == 0) ? whitePrefab : blackPrefab;
        Instantiate(_piece, position, Quaternion.identity);
    }
}
