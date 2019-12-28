using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Utilities
{
  // Items and player are off screen past these points
  public static float LOWER_BOUND = -6f;
  public static float UPPER_BOUND = 8f;
  public static float MAX_HORIZONTAL_BOUND = 11.4f;

  // Items and Player are still on screen at these points\
  public static float LOWER_PLAYER_BOUND = -3.0f;
  public static float UPPER_PLAYER_BOUND = 5.5f;

  // Power up vars
  public static int TRIPLESHOT_POWERUP = 0;
  public static int SPEED_POWERUP = 1;
  public static int SHIELD_POWERUP = 2;
  public static int POWERUP_ARRAY_SIZE = 3;

  // Scene defs
  public static int MENU_SCENE = 0;
  public static int GAME_SCENE = 1;

  // Background location defs
  public static float BACKGROUND_LOWER_OFFSET = 1130f;
  public static float BACKGROUND_UPPER_OFFSET = 3410f;
  public static float BACKGROUND_RELOCATION_DISTANCE = 4520f;

  // Player children definitions
  public static int CHILD_SHIELD_INDEX = 0;
  public static int CHILD_THRUSTER_INDEX = 1;
  public static int CHILD_RIGHTENGINEDAMAGEFADEIN_INDEX = 2;
  public static int CHILD_RIGHTENGINEDAMAGE_INDEX = 3;
  public static int CHILD_LEFTENGINEDAMAGEFADEIN_INDEX = 4;
  public static int CHILD_LEFTENGINEDAMAGE_INDEX = 5;


  public static float RandomX()
  {
    float x = Random.Range(-10.0f, 10.0f);
    return x;
  }

  public static float RandomY()
  {
    float y = Random.Range(8, 12);
    return y;
  }

  public static void ComplainIfGameObjectIsNull(GameObject obj)
  {
    if(obj == null)
    {
      Debug.LogError("WARNING: GameObject " + obj.name + " is null!");
    }
  }
}
