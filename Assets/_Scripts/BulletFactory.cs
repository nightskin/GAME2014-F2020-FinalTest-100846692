using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletFactory 
{
    // step 1. private static instance
    private static BulletFactory m_instance = null;

    // prefab references
    private GameObject regularBullet;
    private GameObject fatBullet;
    private GameObject pulsingBullet;
    private GameObject acornBullet;

    // game controller reference
    private GameController gameController;

    // step 2. make constructor private
    private BulletFactory()
    {
        _Initialize();
    }

    // step 3. make a pubic static creational method for class access
    public static BulletFactory Instance()
    {
        if (m_instance == null)
        {
            m_instance = new BulletFactory();
        }

        return m_instance;
    }

    /// <summary>
    /// This method initializes bullet prefabs
    /// </summary>
    private void _Initialize()
    {
        // 4. create a Resources folder
        // 5. move our Prefabs folder into the Resources folder
        regularBullet = Resources.Load("Prefabs/Bullet") as GameObject;
        fatBullet = Resources.Load("Prefabs/Fat Bullet") as GameObject;
        pulsingBullet = Resources.Load("Prefabs/Pulsing Bullet") as GameObject;
        acornBullet = Resources.Load("Prefabs/Acorn Bullet") as GameObject;

        gameController = GameObject.FindObjectOfType<GameController>();
    }

    /// <summary>
    /// This method creates a bullet of the specified enumeration
    /// </summary>
    /// <param name="type"></param>
    /// <returns> GameObject </returns>
    public GameObject createBullet(BulletType type = BulletType.RANDOM)
    {
        if (type == BulletType.RANDOM)
        {
            var randomBullet = Random.Range(0, 3);
            type = (BulletType) randomBullet;
        }

        GameObject tempBullet = null;
        switch (type)
        {
            case BulletType.REGULAR:
                tempBullet = MonoBehaviour.Instantiate(regularBullet);
                tempBullet.GetComponent<BulletController>().damage = 10;
                break;
            case BulletType.FAT:
                tempBullet = MonoBehaviour.Instantiate(fatBullet);
                tempBullet.GetComponent<BulletController>().damage = 20;
                break;
            case BulletType.PULSING:
                tempBullet = MonoBehaviour.Instantiate(pulsingBullet);
                tempBullet.GetComponent<BulletController>().damage = 30;
                break;
            case BulletType.ACORN:
                tempBullet = MonoBehaviour.Instantiate(acornBullet);
                tempBullet.GetComponent<GrenadeBehaviour>().damage = 10;
                break;
        }

        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }

        tempBullet.transform.parent = gameController.gameObject.transform;
        tempBullet.SetActive(false);

        return tempBullet;
    }
}
