using System.Collections;
using UnityEngine;

public class XYZ : MonoBehaviour {

    private IEnumerator Main() {
        yield return new WaitForSeconds(1);  // 2 seconds wait

        mc.PostToChat("Hello, Unity! (XYZ.cs)");

        // Draw XYZ axes
        yield return StartCoroutine(DrawXYZAxis(0.005f));

        yield return new WaitForSeconds(2.5f);

        mc.PostToChat("good-bye! (XYZ.cs)");
    }

    private IEnumerator DrawXYZAxis(float wait) {
        int AXIS_WIDTH = 40;
        int AXIS_TOP = 64;
        int AXIS_Y_V_ORG = 33;
        int AXIS_BOTTOM = 0;

        string AXIS_BLOCK_X = param.DIAMOND_BLOCK;
        string AXIS_BLOCK_Y = param.SEA_LANTERN;
        string AXIS_BLOCK_Z = param.GOLD_BLOCK;
        string GRASS_BLOCK = param.GRASS;

        // Fill the bottom of the Y-axis with grass (30x30 area)
        mc.PostToChat("Filling the bottom of the Y-axis with grass");
        for (int x = -15; x <= 15; x++) { // Changed range from -10..10 to -15..15
            for (int z = -15; z <= 15; z++) { // Changed range from -10..10 to -15..15
                mc.SetBlock(x, AXIS_BOTTOM, z, GRASS_BLOCK);
                yield return new WaitForSeconds(wait);
            }
        }

        mc.PostToChat("Drawing x-axis from negative to positive region");
        for (int x = -AXIS_WIDTH; x <= AXIS_WIDTH; x++) {
            if (x < 0 && x % 2 == 0) continue; // Skip every other block for negative x
            mc.SetBlock(x, AXIS_Y_V_ORG, 0, AXIS_BLOCK_X);
            yield return new WaitForSeconds(wait);
        }

        mc.PostToChat("Drawing y-axis from bottom to top");
        for (int y = AXIS_BOTTOM; y <= AXIS_TOP; y++) {
            mc.SetBlock(0, y, 0, AXIS_BLOCK_Y);
            yield return new WaitForSeconds(wait);
        }

        mc.PostToChat("Drawing z-axis from negative to positive region");
        for (int z = -AXIS_WIDTH; z <= AXIS_WIDTH; z++) {
            if (z < 0 && z % 2 == 0) continue; // Skip every other block for negative z
            mc.SetBlock(0, AXIS_Y_V_ORG, z, AXIS_BLOCK_Z);
            yield return new WaitForSeconds(wait);
        }
    }

    public MinecraftRemoteSender mc;

    void Start() {
        mc = GameObject.Find("Server").GetComponent<MinecraftRemoteSender>();
        Invoke(nameof(Connect), 1.5f);
    }

    private void Connect() {
        if (mc.ConnectServer() == -1) {
            print("Couldn't connect to the server, good-bye.");
        } else {
            StartCoroutine(Main());
        }
    }
}