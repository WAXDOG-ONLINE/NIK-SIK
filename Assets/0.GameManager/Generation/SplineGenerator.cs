using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;


public class SplineGenerator : MonoBehaviour
{
    public SplineContainer hallways;
    public Transform doorwayOne;
    public Transform doorwayTwo;

    public Transform doorwayThree;
    public Transform doorwayFour;
    public NavMeshSurface navMesh;

    
    public List<Room> rooms;
    public List<Transform> doorways = new List<Transform>();

    

    
    // Start is called before the first frame update
    void Start()
    {





//pairDoorways(rooms);



        /*
        //create a new hallway
        Spline hallway1 = hallways.AddSpline();

        
        //calculate tangents
        
        //create hallway entry point from doorway refrence
        //set tangent such that it is pointing towards the halway and in line with the other doorway
        Vector3 tangent = new Vector3(0 ,0,doorwayTwo.position.z - doorwayOne.position.z );
        BezierKnot firstKnot = 
        new BezierKnot(doorwayOne.position, tangent, tangent, Quaternion.Euler(new Vector3 (0, doorwayOne.eulerAngles.y,0)));
          hallway1.Add(firstKnot);

        //create hallway exit point from doorway refrence
         tangent = new Vector3(0,0,doorwayOne.position.x -doorwayTwo.position.x );
        BezierKnot secondKnot = 
        new BezierKnot(doorwayTwo.position,tangent,tangent,Quaternion.Euler(new Vector3 (0, doorwayTwo.eulerAngles.y,0)));
        hallway1.Add(secondKnot);

        //recreate hallway meshes
       
        Destroy(transform.GetComponent<MeshCollider>());
        transform.AddComponent<MeshCollider>();
        */
        
    }

    
    public void pairDoorways(List<Room> rooms){

            //Instead of using a foreach loop, 
            //use a for loop and iterate backwards. 
            //That way you're removing objects "behind" you in the iterator, and you won't run into index problems.





        //add all doorways from all rooms to a mega list of doorways
        //loop through all rooms
        foreach(Room roomAlpha in rooms){
            Transform doorwayToPair ;
            Transform pairedDoorwayCandidate;
            Transform pairedDoorway = null;
            Room roomToRemoveFrom = null;
            float magnitudePrevious = 100000;
            List<Transform> roomDoorways = roomAlpha.doorways;
           //make a new list of all rooms minus the room we are starting from
            List<Room> roomsToCheck = new List<Room>(rooms);
            roomsToCheck.Remove(roomAlpha);
            //iterate over doors in selected room and find them each a pair
            
            foreach(Transform doorAlpha in roomAlpha.doorways){
               doorwayToPair = doorAlpha;

            
                //iterate over remaining rooms and their doors and find the one with the smalest distance
                //need to check for intersections with rooms somehow???
                //maybe add each spline here and check
               magnitudePrevious = 10000;
               foreach(Room room in roomsToCheck){
                
                    foreach(Transform doorway in room.doorways){
                            pairedDoorwayCandidate = doorway;       
                            float magnitude = (doorwayToPair.position - pairedDoorwayCandidate.position).magnitude;
                            if(magnitude < magnitudePrevious){
                                pairedDoorway = pairedDoorwayCandidate;
                                 roomToRemoveFrom = room;
                                 magnitudePrevious = magnitude;
                            }
                        
                    }
               }


               //add paired doorway and working doorway to the doorway list
               //remove them from their original rooms
               if(roomToRemoveFrom != null && pairedDoorway != null){
            //remove end doorway from its room
            print(roomToRemoveFrom);
            print(pairedDoorway);
               roomToRemoveFrom.doorways.Remove(pairedDoorway);
               //remove starting doorway from its room
               
               //add doorways to list
               doorways.Add(doorwayToPair);
                doorways.Add(pairedDoorway);
               }

              
            
            


        }
        
        roomAlpha.doorways.Clear(); 

        //maybe run over list of rooms instead so that you dont need to kee ptrack of which room door belongs too
        }
        //find a pair for each doorway
        generateHallways(doorways);


    }
//send this function a list of doorways, each pair of doorways will form a hallway
    public void generateHallways(List<Transform> doorways){
        for(int i = 0; i < doorways.Count;){
            
            doorwayOne = doorways[i];
            doorwayTwo = doorways[i+1];

        Spline workingHallway = hallways.AddSpline();

        
        //calculate tangents
        
        //create hallway entry point from doorway refrence
        //set tangent such that it is pointing towards the halway and in line with the other doorway
        //Vector3 tangent = new Vector3(0 ,0,doorwayTwo.position.z - doorwayOne.position.z );
        Vector3 tangent = new Vector3(0,0,3);
        BezierKnot firstKnot = 
        new BezierKnot(doorwayOne.position, tangent, tangent, Quaternion.Euler(new Vector3 (0, doorwayOne.eulerAngles.y,0)));
          workingHallway.Add(firstKnot);

        //create hallway exit point from doorway refrence
        // tangent = new Vector3(0,0,doorwayOne.position.x -doorwayTwo.position.x );
        BezierKnot secondKnot = 
        new BezierKnot(doorwayTwo.position,tangent,tangent,Quaternion.Euler(new Vector3 (0, doorwayTwo.eulerAngles.y,0)));
        workingHallway.Add(secondKnot);



            i = i+ 2;
        }
         Destroy(transform.GetComponent<MeshCollider>());
        transform.AddComponent<MeshCollider>();
        navMesh.BuildNavMesh();


    }


public void fixIntersections(){



}
    // Update is called once per frame
    void Update()
    {
        
    }
}
