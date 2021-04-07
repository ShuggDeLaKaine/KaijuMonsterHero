using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    //vehicle pooling - array of GameObjects prefabs, Dictionary for pool, Int for pool size & Transform of pool position.
    public Dictionary<GameObject, bool> vehiclePool = new Dictionary<GameObject, bool>();
    public GameObject[] vehiclePrefabs;
    public Transform vehiclePoolPosition;
    public int vehPoolSize;

    //vehicle spawning = list of spawnpoints, gameobject of current spawn points, ints for index in list and numbers of active relevant objects in scene.
    public List<GameObject> vehSpawnPoints = new List<GameObject>();
    public GameObject vehCurrentSpawnPoint;
    private GameObject selectedVehicle;     //refenence to GameObject for currently selected vehicle prefab.
    public GameObject spawnVehiclePosition;
    private int vehCurIndex;
    private int vehPrevIndex;
    public int vehMaxActive;
    public int vehCurActive;

    //people pooling - all that is required (same things as vehicle pooling).
    public Dictionary<GameObject, bool> peoplePool = new Dictionary<GameObject, bool>();
    public GameObject[] peoplePrefabs;
    public Transform peoplePoolPosition;
    public int peoplePoolSize;

    //people spawing - all that is required (pretty much the same as vehicle spawing).
    public List<GameObject> ppSpawnPoints = new List<GameObject>();
    public GameObject ppCurrentSpawnPoint;
    private GameObject selectedPerson;    
    public GameObject spawnPersonPosition;
    private int ppCurIndex;
    private int ppPrevInd;
    public int ppMaxActive;
    public int ppCurActive;

    //vehicle explosion FX pooling & spawning.
    public Dictionary<GameObject, bool> carExFXPool = new Dictionary<GameObject, bool>();
    public GameObject[] carExFXPrefabs;
    public GameObject selectedCarExFX;
    public Transform carExFXPoolPosition;
    public int carExFXPoolSize;
    //private int carExCurIndex;
    //private int carExPrevIndex;

    //building destruction FX pooling & spawning.
    public Dictionary<GameObject, bool> buildExFXPool = new Dictionary<GameObject, bool>();
    public GameObject[] buildExFXPrefabs;
    public GameObject selectedBuildExFX;
    public Transform buildExFXPoolPosition;
    public int buildExFXPoolSize;
    //private int buildExCurIndex;
    //private int buildExPrevIndex;

    //building damage FX pooling & spawning.
    public Dictionary<GameObject, bool> buildDAMFXPool = new Dictionary<GameObject, bool>();
    public GameObject[] buildDAMFXPrefabs;
    public GameObject selectedBuildDAMFX;
    public Transform buildDAMFXPoolPosition;
    public int buildDAMFXPoolSize;
    //private int buildDAMCurIndex;
    //private int buildDAMPrevIndex;

    //scenery destruction FX pooling & spawning.
    public Dictionary<GameObject, bool> scnDesFXPool = new Dictionary<GameObject, bool>();
    public GameObject[] scnDesFXPrefabs;
    public GameObject selectedScnDesFX;
    public Transform scnDesFXPoolPosition;
    public int scnDesFXPoolSize;
    //private int scnDesFXCurIndex;
    //private int scnDesFXPrevIndex;

    //factory destruction FX pooling & spawning.
    public Dictionary<GameObject, bool> factDesFXPool = new Dictionary<GameObject, bool>();
    public GameObject[] factDesFXPrefabs;
    public GameObject selectedFactDesFX;
    public Transform factDesFXPoolPosition;
    public int factDesFXPoolSize;
    //private int factDesFXCurIndex;
    //private int factDesFXPrevIndex;

    //people destruction FX pooling & spawning.
    public Dictionary<GameObject, bool> ppDesFXPool = new Dictionary<GameObject, bool>();
    public GameObject[] ppDesFXPrefabs;
    public GameObject selectedPpDesFX;
    public Transform ppDesFXPoolPosition;
    public int ppDesFXPoolSize;
    //private int ppDesFXCurIndex;
    //private int ppDesFXPrevIndex;

    //building damage SoundFX pooling & spawning.
    public Dictionary<GameObject, bool> sfxBuildDamPool = new Dictionary<GameObject, bool>();
    public GameObject[] sfxBuildDamPrefabs;
    public GameObject selectedSFXBuildDam;
    public Transform sfxBuildDamPoolPosition;
    public int sfxBuildDamPoolSize;
    //private int sfxBuildDamCurIndex;
    //private int sfxBuildDamPrevIndex;

    //building destruction SoundFX pooling & spawning.
    public Dictionary<GameObject, bool> sfxBuildDesPool = new Dictionary<GameObject, bool>();
    public GameObject[] sfxBuildDesPrefabs;
    public GameObject selectedSFXBuildDes;
    public Transform sfxBuildDesPoolPosition;
    public int sfxBuildDesPoolSize;
    //private int sfxBuildDesCurIndex;
    //private int sfxBuildDesPrevIndex;

    //vehicle destruction SoundFX pooling & spawning.
    public Dictionary<GameObject, bool> sfxVehDesPool = new Dictionary<GameObject, bool>();
    public GameObject[] sfxVehDesPrefabs;
    public GameObject selectedSFXVehDes;
    public Transform sfxVehDesPoolPosition;
    public int sfxVehDesPoolSize;
    //private int sfxVehDesCurIndex;
    //private int sfxVehDesPrevIndex;

    //people destruction SoundFX pooling & spawning.
    public Dictionary<GameObject, bool> sfxPpDesPool = new Dictionary<GameObject, bool>();
    public GameObject[] sfxPpDesPrefabs;
    public GameObject selectedSFXPpDes;
    public Transform sfxPpDesPoolPosition;
    public int sfxPpDesPoolSize;
    //private int sfxPpDesCurIndex;
    //private int sfxPpDesPrevIndex;

    //scenery destruction SoundFX pooling & spawning.
    public Dictionary<GameObject, bool> sfxScnDesPool = new Dictionary<GameObject, bool>();
    public GameObject[] sfxScnDesPrefabs;
    public GameObject selectedSFXScnDes;
    public Transform sfxScnDesPoolPosition;
    public int sfxScnDesPoolSize;
    //private int sfxScnDesCurIndex;
    //private int sfxScnDesPrevIndex;

    //factory destruction SoundFX pooling & spawning.
    public Dictionary<GameObject, bool> sfxFactDesPool = new Dictionary<GameObject, bool>();
    public GameObject[] sfxFactDesPrefabs;
    public GameObject selectedSFXFactDes;
    public Transform sfxFactDesPoolPosition;
    public int sfxFactDesPoolSize;


    void Start()
    {
        CreateAllPools();
        InitiateSpawn();
    }

    void Update()
    {
        RefillAllPools();
        RefillScene();
    }

    //function to set up spawn locations for vehicles and people.
    private void InitiateSpawn()
    {
        //setting numbers of max and current vehicles and people in scene.
        vehMaxActive = 5;
        vehCurActive = 0;
        ppMaxActive = 5;
        ppCurActive = 0;

        //randomise first vehicle spawn point.
        vehCurIndex = Random.Range(0, (vehSpawnPoints.Count));
        vehPrevIndex = vehCurIndex;

        //randomise first people spawn point.
        ppCurIndex = Random.Range(0, (ppSpawnPoints.Count));
        ppPrevInd = ppCurIndex;
    }

    private void CreateAllPools()
    {
        //setting the pool sizes.
        vehPoolSize = 50;
        peoplePoolSize = 50;
        carExFXPoolSize = 50;
        buildExFXPoolSize = 50;
        buildDAMFXPoolSize = 50;
        scnDesFXPoolSize = 50;
        factDesFXPoolSize = 50;
        ppDesFXPoolSize = 50;
        sfxBuildDamPoolSize = 50;
        sfxBuildDesPoolSize = 50;
        sfxVehDesPoolSize = 50;
        sfxPpDesPoolSize = 50;
        sfxScnDesPoolSize = 50;
        sfxFactDesPoolSize = 50;

        //create & fill vehicle pool
        CreatePool(vehPoolSize, vehiclePrefabs, vehiclePoolPosition, vehiclePool);
        //create & fill people pool
        CreatePool(peoplePoolSize, peoplePrefabs, peoplePoolPosition, peoplePool);
        //create & fill car destruction FX pool
        CreatePool(carExFXPoolSize, carExFXPrefabs, carExFXPoolPosition, carExFXPool);
        //create & fill building destruction FX pool
        CreatePool(buildExFXPoolSize, buildExFXPrefabs, buildExFXPoolPosition, buildExFXPool);
        //...building damage FX pool
        CreatePool(buildDAMFXPoolSize, buildDAMFXPrefabs, buildDAMFXPoolPosition, buildDAMFXPool);
        //...scene damage FX pool
        CreatePool(scnDesFXPoolSize, scnDesFXPrefabs, scnDesFXPoolPosition, scnDesFXPool);
        //...factory damage FX pool
        CreatePool(factDesFXPoolSize, factDesFXPrefabs, factDesFXPoolPosition, factDesFXPool);
        //...people destruction FX pool
        CreatePool(ppDesFXPoolSize, ppDesFXPrefabs, ppDesFXPoolPosition, ppDesFXPool);
        //SFX building damage
        CreatePool(sfxBuildDamPoolSize, sfxBuildDamPrefabs, sfxBuildDamPoolPosition, sfxBuildDamPool);
        //SFX building destroy
        CreatePool(sfxBuildDesPoolSize, sfxBuildDesPrefabs, sfxBuildDesPoolPosition, sfxBuildDesPool);
        //SFX vehicle destroy
        CreatePool(sfxVehDesPoolSize, sfxVehDesPrefabs, sfxVehDesPoolPosition, sfxVehDesPool);
        //SFX people destroy
        CreatePool(sfxPpDesPoolSize, sfxPpDesPrefabs, sfxPpDesPoolPosition, sfxPpDesPool);
        //SFX scenery destroy
        CreatePool(sfxScnDesPoolSize, sfxScnDesPrefabs, sfxScnDesPoolPosition, sfxScnDesPool);
        //SFX factory destroy
        CreatePool(sfxFactDesPoolSize, sfxFactDesPrefabs, sfxFactDesPoolPosition, sfxFactDesPool);
    }

    //function to create a pool of objects to the required pool size.
    private void CreatePool(int poolSize, GameObject[] prefabType, Transform transType, Dictionary<GameObject, bool> dictType)
    {
        //loop to instanitate prefab clones at temperory position and fill the dictionary.
        for (int i = 0; i < poolSize; i++)
        {
            //instaniate objects at temporary position.
            GameObject obj = (GameObject)Instantiate(prefabType[Random.Range(0, prefabType.Length)], transType.position, Quaternion.identity);
            //add this game object to the dictionary with its ready state = true
            dictType.Add(obj, true);
            //set the object to inactive
            obj.SetActive(false);
        }
    }

    //function to refill a pool of objects to desired pool size. 
    private void RefillPool(int poolsize, GameObject[] prefabType, Transform transType, Dictionary<GameObject, bool> dictType)
    {
        int dicLength = dictType.Count;
        int toRefill = poolsize - dicLength;

        //if the pool is empty, better refill it!
        if (dicLength == 0)
        {
            //loop to instanitate prefab clones at temperory position and fill the dictionary.
            for (int i = 0; i < toRefill; i++)
            {
                //instaniate objects at temporary position.
                GameObject obj = (GameObject)Instantiate(prefabType[Random.Range(0, prefabType.Length)], transType.position, Quaternion.identity);
                //add this game object to the dictionary with its ready state = true
                dictType.Add(obj, true);
                //set the object to inactive
                obj.SetActive(false);
            }
        }
    }

    //function to refill all the pools.
    private void RefillAllPools()
    {
        //refill vehicle pool
        RefillPool(vehPoolSize, vehiclePrefabs, vehiclePoolPosition, vehiclePool);
        //refill people pool
        RefillPool(peoplePoolSize, peoplePrefabs, peoplePoolPosition, peoplePool);
        //refill car destruction FX pool
        RefillPool(carExFXPoolSize, carExFXPrefabs, carExFXPoolPosition, carExFXPool);
        //refill building destruction FX pool
        RefillPool(buildExFXPoolSize, buildExFXPrefabs, buildExFXPoolPosition, buildExFXPool);
        //... building damage FX pool
        RefillPool(buildDAMFXPoolSize, buildDAMFXPrefabs, buildDAMFXPoolPosition, buildDAMFXPool);
        //... scenery destruction FX pool
        RefillPool(scnDesFXPoolSize, scnDesFXPrefabs, scnDesFXPoolPosition, scnDesFXPool);
        //... factory destruction FX pool
        RefillPool(factDesFXPoolSize, factDesFXPrefabs, factDesFXPoolPosition, factDesFXPool);
        //... people destruction FX pool
        RefillPool(ppDesFXPoolSize, ppDesFXPrefabs, ppDesFXPoolPosition, ppDesFXPool);
        //SFX building damage
        RefillPool(sfxBuildDamPoolSize, sfxBuildDamPrefabs, sfxBuildDamPoolPosition, sfxBuildDamPool);
        //SFX building destroy
        RefillPool(sfxBuildDesPoolSize, sfxBuildDesPrefabs, sfxBuildDesPoolPosition, sfxBuildDesPool);
        //SFX vehicle destroy
        RefillPool(sfxVehDesPoolSize, sfxVehDesPrefabs, sfxVehDesPoolPosition, sfxVehDesPool);
        //SFX people destroy
        RefillPool(sfxPpDesPoolSize, sfxPpDesPrefabs, sfxPpDesPoolPosition, sfxPpDesPool);
        //SFX scenery destroy
        RefillPool(sfxScnDesPoolSize, sfxScnDesPrefabs, sfxScnDesPoolPosition, sfxPpDesPool);
        //SFX factory destroy
        RefillPool(sfxFactDesPoolSize, sfxFactDesPrefabs, sfxFactDesPoolPosition, sfxFactDesPool);
    }

    //function to refill the vehicles and people in the scene.
    private void RefillScene()
    {
        //refill scene with vehicles.
        VehicleCheckRefillScene();
        //refill scene with people.
        PeopleCheckRefillScene();
    }

    //function to check if refill required and then spawn necessary objects.
    private void VehicleCheckRefillScene()
    {
        //check if the current number is below the desired number. 
        if (vehCurActive < vehMaxActive)
        {
            StartCoroutine(VehicleSpawnSequence());
        }
    }

    //function to check if refill required and then spawn necessary objects.
    private void PeopleCheckRefillScene()
    {
        //check if the current number is below the desired number. 
        if (ppCurActive < ppMaxActive)
        {
            StartCoroutine(PeopleSpawnSequence());
        }
    }

    //
    IEnumerator VehicleSpawnSequence()
    {
        RandomVehicleSpawnLocation();
        SpawnVehiclePrefab();
        yield return null;
    }

    //
    IEnumerator PeopleSpawnSequence()
    {
        RandomPeopleSpawnLocation();
        SpawnPeoplePrefab();
        yield return null;
    }

    //function to randomly selected spawn location to move object to.
    private void RandomVehicleSpawnLocation()
    {
        vehCurIndex = Random.Range(0, (vehSpawnPoints.Count));

        if (vehCurIndex == vehPrevIndex)
        {
            vehPrevIndex = vehCurIndex;
            vehCurIndex = Random.Range(0, (vehSpawnPoints.Count));
            vehCurrentSpawnPoint = vehSpawnPoints[vehCurIndex];
        }
        else
        {
            vehCurrentSpawnPoint = vehSpawnPoints[vehCurIndex];
        }
    }

    //function to randomly selected people spawn point.
    private void RandomPeopleSpawnLocation()
    {
        ppCurIndex = Random.Range(0, (ppSpawnPoints.Count));

        if (ppCurIndex == ppPrevInd)
        {
            ppPrevInd = ppCurIndex;
            ppCurIndex = Random.Range(0, (ppSpawnPoints.Count));
            ppCurrentSpawnPoint = ppSpawnPoints[ppCurIndex];
        }
        else
        {
            ppCurrentSpawnPoint = ppSpawnPoints[ppCurIndex];
        }
    }

    //function to take a vehicle object out of the pool and spawn enabled at a spawn location.
    private void SpawnVehiclePrefab()
    {
        //first check that there is a currentSpawnPoint assigned.
        if(vehCurrentSpawnPoint != null && vehCurrentSpawnPoint.GetComponent<Spawner>().canSpawn == true)
        {
            //layered foreach and if loops checking that a prefab clone is ready, found & then set to selectedVechicle GameObject.
            foreach (KeyValuePair<GameObject, bool> obj in vehiclePool)
            {
                //check if a ready prefab is found
                if(obj.Value == true)
                {
                    //store this prefab as the selectedPrefab.
                    selectedVehicle = obj.Key;
                    //break out of the loop.
                    break;
                }
            }
            float axis = vehCurrentSpawnPoint.transform.localEulerAngles.y;
            //set rotation so vehicle comes out of the 'north/upwards' direction of the spawn object.
            selectedVehicle.transform.Rotate(Vector3.up, axis, Space.Self);
            //set as ready = false for prefab in the dictionary
            vehiclePool[selectedVehicle] = false;
            //set active of this selected vehicle object to true
            selectedVehicle.SetActive(true);
            //change transform position of selected vehicle to the currentSpawnPoint object transform.position
            selectedVehicle.transform.position = vehCurrentSpawnPoint.transform.position;
            //Remove the selected vehicle prefab from the vehicle pool dictionary.
            vehiclePool.Remove(selectedVehicle);
            //add 1 to the current active vehicles in scene. 
            vehCurActive++;
            //tell the spawner of the spawn location to switch its following bools.
            vehCurrentSpawnPoint.GetComponent<Spawner>().justSpawned = true;
            vehCurrentSpawnPoint.GetComponent<Spawner>().canSpawn = false;
        }
        else 
        {
            return;
        }
    }

    //function to take a people object out of the pool and spawn enabled at a spawn location.
    private void SpawnPeoplePrefab()
    {
        //first check that there is a currentSpawnPoint assigned.
        if (ppCurrentSpawnPoint != null && ppCurrentSpawnPoint.GetComponent<Spawner>().canSpawn == true)
        {
            //layered foreach and if loops checking that a prefab clone is ready, found & then set to selectedVechicle GameObject.
            foreach (KeyValuePair<GameObject, bool> obj in peoplePool)
            {
                //check if a ready prefab is found
                if (obj.Value == true)
                {
                    //store this prefab as the selectedPrefab.
                    selectedPerson = obj.Key;
                    //break out of the loop.
                    break;
                }
            }
            float axis = ppCurrentSpawnPoint.transform.localEulerAngles.y;
            //set rotation so vehicle comes out of the 'north/upwards' direction of the spawn object.
            selectedPerson.transform.Rotate(Vector3.up, axis, Space.Self);
            //set as ready = false for prefab in the dictionary
            peoplePool[selectedPerson] = false;
            //set active of this selected person object to true
            selectedPerson.SetActive(true);
            //issues with NavMeshAgent when using change transform, so using .Warp function.
            selectedPerson.GetComponent<NavMeshAgent>().Warp(ppCurrentSpawnPoint.transform.position);
            //Remove the selected vehicle prefab from the people pool dictionary.
            peoplePool.Remove(selectedPerson);
            //add 1 to the current active people in scene. 
            ppCurActive++;
            //tell the spawner of the spawn location to switch its following bools.
            ppCurrentSpawnPoint.GetComponent<Spawner>().justSpawned = true;
            ppCurrentSpawnPoint.GetComponent<Spawner>().canSpawn = false;
        }
        else
        {
            return;
        }
    }

    //function to take a FX object out of the relevant pool and spawn enabled at object location required.
    public void SpawnFXs(Dictionary<GameObject, bool> dictPool, GameObject selectedObject, GameObject positionObject)
    {
        //layered foreach and if loops checking that a prefab clone is ready, found & then set to selectedVechicle GameObject.
        foreach (KeyValuePair<GameObject, bool> obj in dictPool)
        {
            //check if a ready prefab is found
            if (obj.Value == true)
            {
                //store this prefab as the selectedPrefab.
                selectedObject = obj.Key;
                //objectPosition = positionObject.transform;
                //break out of the loop.
                break;
            }
        }
        
        if(selectedObject != null)
        { 
            //set as ready = false for prefab in the dictionary
            dictPool[selectedObject] = false;
            //change transform position of selected object to the currentSpawnPoint object transform.position
            selectedObject.transform.position = positionObject.transform.position;
            //set active of this selected object object to true
            selectedObject.SetActive(true);
            //Remove the selected object prefab from its pool dictionary.
            dictPool.Remove(selectedObject);
        }
    }

    //function to replace DestroyThis, moves object back to its relevant pool.
    public void BackToPool(Dictionary<GameObject, bool> poolDict, Transform poolTransform, GameObject currentObject)
    {
        //add current object to relevant pool Dictionary with it's ready state set to true
        poolDict.Add(currentObject, true);
        //deactive currentobject.
        currentObject.SetActive(false);

        /*
        currentObject.SetActiveRecursively(false);
        for(int i = 0; i < currentObject.transform.childCount; i++)
        {
            currentObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        */

        //move deactive currentobject to pool position.
        currentObject.transform.position = poolTransform.position;
    }

}
