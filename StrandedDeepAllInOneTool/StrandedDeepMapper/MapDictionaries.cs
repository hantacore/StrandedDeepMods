using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrandedDeepMapper
{
    public class MapDictionaries
    {
        private static Dictionary<string, string> namesDictionary;

        public static Dictionary<string, string> NamesDictionary
        {
            get
            {
                return namesDictionary;
            }
        }

        private static Dictionary<string, string> allDictionary;

        public static Dictionary<string, string> AllDictionary
        {
            get
            {
                return allDictionary;
            }
        }

        static MapDictionaries()
        {
            allDictionary = new Dictionary<string, string>();

            allDictionary.Add("BAT","BAT");
            allDictionary.Add("SEAGULL", "SEAGULL");
            allDictionary.Add("BOAR", "DRAW|BOAR");
            allDictionary.Add("BOAR_RAGDOLL", "DRAW|BOAR");
            allDictionary.Add("BOAR_SPAWNER", "DRAW|BOAR");
            allDictionary.Add("HOG", "DRAW|BOAR");
            allDictionary.Add("HOG_RAGDOLL", "DRAW|BOAR");
            allDictionary.Add("HOG_SPAWNER", "DRAW|BOAR");
            allDictionary.Add("CRAB", "DRAW|CRAB");
            allDictionary.Add("CRAB_SPAWNER", "DRAW|CRAB");
            allDictionary.Add("GIANT_CRAB_SPAWNER", "DRAW|CRAB");
            allDictionary.Add("GIANT_CRAB_RAGDOLL", "DRAW|CRAB");
            allDictionary.Add("GIANT_CRAB", "CRAB");
            allDictionary.Add("ARCHER", "FISH");
            allDictionary.Add("CLOWN_TRIGGERFISH", "FISH");
            allDictionary.Add("COD", "FISH");
            allDictionary.Add("DISCUS", "FISH");
            allDictionary.Add("LIONFISH", "FISH");
            allDictionary.Add("LIONFISH_RAGDOLL", "FISH");
            allDictionary.Add("PILCHARD", "FISH");
            allDictionary.Add("PILCHARD_RAGDOLL", "FISH");
            allDictionary.Add("SARDINE", "FISH");
            allDictionary.Add("SARDINE_RAGDOLL", "FISH");
            allDictionary.Add("STING_RAY", "DRAW|STINGRAY");
            allDictionary.Add("STING_RAY_RAGDOLL", "DRAW|STINGRAY");
            allDictionary.Add("GROUPER", "FISH");
            allDictionary.Add("GROUPER_RAGDOLL", "FISH");

            allDictionary.Add("MARLIN", "DRAW|MARLIN");
            allDictionary.Add("MARLIN_RAGDOLL", "DRAW|MARLIN");

            allDictionary.Add("WHALE", "DRAW|WHALE");

            allDictionary.Add("SHARK_WHITE", "DRAW|SHARK");
            allDictionary.Add("SHARK_REEF", "DRAW|SHARK");
            allDictionary.Add("SHARK_TIGER", "DRAW|SHARK");
            allDictionary.Add("SHARK - HAMMERHEAD", "DRAW|SHARK");
            allDictionary.Add("SHARK - GREAT WHITE", "DRAW|SHARK");

            allDictionary.Add("SHARK_GREAT WHITE_RAGDOLL", "DRAW|SHARK");
            allDictionary.Add("SHARK_REEF_RAGDOLL", "DRAW|SHARK");
            allDictionary.Add("SHARK_TIGER_SHARK_RAGDOLL", "DRAW|SHARK");
            allDictionary.Add("SHARK_HAMMERHEAD_RAGDOLL", "DRAW|SHARK");
            allDictionary.Add("PATROL_GREATWHITE", "DRAW|SHARK");
            allDictionary.Add("PATROL_MARLIN", "DRAW|MARLIN");
            allDictionary.Add("PATROL_REEFSHARK", "DRAW|SHARK");
            allDictionary.Add("PATROL_TIGERSHARK", "DRAW|SHARK");
            allDictionary.Add("PATROL_HAMMERHEAD", "DRAW|SHARK");

            allDictionary.Add("HIDESPOT_SNAKE", "DRAW|SNAKE");
            allDictionary.Add("SNAKE", "DRAW|SNAKE");
            allDictionary.Add("SNAKE_RAGDOLL", "DRAW|SNAKE");
            allDictionary.Add("SNAKE_SPAWNER", "DRAW|SNAKE");

            allDictionary.Add("BARREL", "DRAW|BARREL");
            allDictionary.Add("BARREL_PILE", "DRAW|BARREL");
            allDictionary.Add("BRICK_ARCH", "");
            allDictionary.Add("BRICK_FLOOR", "");
            allDictionary.Add("BRICK_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("BRICK_PANEL_STATIC", "");
            allDictionary.Add("BRICK_ROOF_CAP", "");
            allDictionary.Add("BRICK_ROOF_CORNER", "");
            allDictionary.Add("BRICK_ROOF_MIDDLE", "");
            allDictionary.Add("BRICK_ROOF_WEDGE", "");
            allDictionary.Add("BRICK_STEPS", "");
            allDictionary.Add("BRICK_WALL_HALF", "");
            allDictionary.Add("BRICK_WALL_WINDOW", "");
            allDictionary.Add("BRICK_WEDGE_FLOOR", "");
            allDictionary.Add("BRICK_WEDGE_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("BRICKS", "DRAW|RESOURCE");
            allDictionary.Add("BUOYBALL", "DRAW|BUOY");
            allDictionary.Add("BUOYBALL_PILE", "DRAW|BUOY");
            allDictionary.Add("CORRUGATED_ARCH", "");
            allDictionary.Add("CORRUGATED_DOOR", "");
            allDictionary.Add("CORRUGATED_FLOOR", "");
            allDictionary.Add("CORRUGATED_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("CORRUGATED_PANEL_STATIC", "");
            allDictionary.Add("CORRUGATED_STEPS", "");
            allDictionary.Add("CORRUGATED_WALL_HALF", "");
            allDictionary.Add("CORRUGATED_WALL_WINDOW", "");
            allDictionary.Add("CORRUGATED_WEDGE_FLOOR", "");
            allDictionary.Add("CORRUGATED_WEDGE_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("DRIFTWOOD_ARCH", "");
            allDictionary.Add("DRIFTWOOD_DOOR", "");
            allDictionary.Add("DRIFTWOOD_FLOOR", "");
            allDictionary.Add("DRIFTWOOD_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("DRIFTWOOD_PANEL_STATIC", "");
            allDictionary.Add("DRIFTWOOD_STEPS", "");
            allDictionary.Add("DRIFTWOOD_WALL_HALF", "");
            allDictionary.Add("DRIFTWOOD_WALL_WINDOW", "");
            allDictionary.Add("DRIFTWOOD_WEDGE_FLOOR", "");
            allDictionary.Add("DRIFTWOOD_WEDGE_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("GYRO_BASE_1", "GYRO");
            allDictionary.Add("GYRO_COCKPIT_4", "GYRO");
            allDictionary.Add("GYRO_MOTOR_3", "GYRO");
            allDictionary.Add("GYRO_ROTORS_5", "GYRO");
            allDictionary.Add("GYRO_SEAT_2", "GYRO");
            allDictionary.Add("GYRO_STRUCTURE", "GYRO");
            allDictionary.Add("RAFT_OUTRIGGER", "");
            allDictionary.Add("WOOD_CANOE", "");
            allDictionary.Add("WOOD_RAFT", "");
            allDictionary.Add("PLANK_ARCH", "");
            allDictionary.Add("PLANK_DOOR", "");
            allDictionary.Add("PLANK_FLOOR", "");
            allDictionary.Add("PLANK_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("PLANK_PANEL_STATIC", "");
            allDictionary.Add("PLANK_STEPS", "");
            allDictionary.Add("PLANK_WALL_HALF", "");
            allDictionary.Add("PLANK_WALL_WINDOW", "");
            allDictionary.Add("PLANK_WEDGE_FLOOR", "");
            allDictionary.Add("PLANK_WEDGE_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("RAFT_BASE_BALLS", "");
            allDictionary.Add("RAFT_BASE_BARREL", "");
            allDictionary.Add("RAFT_BASE_TYRE", "");
            allDictionary.Add("RAFT_BASE_WOOD_BUNDLE", "");
            allDictionary.Add("RAFT_FLOOR_CORRUGATED", "");
            allDictionary.Add("RAFT_FLOOR_DRIFTWOOD", "");
            allDictionary.Add("RAFT_FLOOR_PLANK", "");
            allDictionary.Add("RAFT_FLOOR_STEEL", "");
            allDictionary.Add("RAFT_FLOOR_WOOD", "");
            allDictionary.Add("RAFT_FLOOR_CLAY", "");
            allDictionary.Add("RAFT_CANOPY", "");
            allDictionary.Add("SCRAP_CORRUGATED", "DRAW|RESOURCE");
            allDictionary.Add("SCRAP_PLANK", "DRAW|RESOURCE");
            allDictionary.Add("SHIPPING_CONTAINER", "DRAW|CONTAINER");
            allDictionary.Add("SHIPPING_CONTAINER_1", "DRAW|CONTAINER");
            allDictionary.Add("SHIPPING_CONTAINER_2", "DRAW|CONTAINER");
            allDictionary.Add("SHIPPING_CONTAINER_3", "DRAW|CONTAINER");
            allDictionary.Add("SHIPPING_CONTAINER_DOOR", "DRAW|RESOURCE");
            allDictionary.Add("SHIPPING_CONTAINER_DOOR_STATIC", "");
            allDictionary.Add("SHIPPING_CONTAINER_FLOOR", "DRAW|RESOURCE");
            allDictionary.Add("SHIPPING_CONTAINER_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("SHIPPING_CONTAINER_PANEL", "DRAW|RESOURCE");
            allDictionary.Add("SHIPPING_CONTAINER_PANEL_STATIC", "");
            allDictionary.Add("SHIPPING_CONTAINER_STEPS", "");
            allDictionary.Add("SHIPPING_CONTAINER_WEDGE_FLOOR", "");
            allDictionary.Add("SHIPPING_CONTAINER_WEDGE_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("STEEL_DOOR", "");
            allDictionary.Add("STEEL_STEPS", "");
            allDictionary.Add("STRUCTURE", "");
            allDictionary.Add("STRUCTURE_RAFT", "DRAW|RAFT");
            allDictionary.Add("STRUCTURE_SMALL", "");
            allDictionary.Add("TARP_PANEL", "DRAW|RESOURCE");
            allDictionary.Add("TARP_PANEL_STATIC", "");
            allDictionary.Add("TYRE", "DRAW|TIRE");
            allDictionary.Add("TYRE_PILE", "DRAW|TIRE");
            allDictionary.Add("VEHICLE_HELICOPTER", "DRAW|HELI");
            allDictionary.Add("VEHICLE_LIFERAFT", "DRAW|RAFT");
            allDictionary.Add("VEHICLE_MOTOR", "");
            allDictionary.Add("VEHICLE_SAIL", "");
            allDictionary.Add("WOOD_ARCH", "");
            allDictionary.Add("WOOD_DOOR", "");
            allDictionary.Add("WOOD_FLOOR", "");
            allDictionary.Add("WOOD_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("WOOD_PANEL_STATIC", "");
            allDictionary.Add("WOOD_ROOF_CAP", "");
            allDictionary.Add("WOOD_ROOF_CORNER", "");
            allDictionary.Add("WOOD_ROOF_MIDDLE", "");
            allDictionary.Add("WOOD_ROOF_WEDGE", "");
            allDictionary.Add("WOOD_STEPS", "");
            allDictionary.Add("WOOD_WALL_HALF", "");
            allDictionary.Add("WOOD_WALL_WINDOW", "");
            allDictionary.Add("WOOD_WEDGE_FLOOR", "");
            allDictionary.Add("WOOD_WEDGE_FOUNDATION", "DRAW|BUILDING");
            allDictionary.Add("CONTAINER_CONSOLE", "");
            allDictionary.Add("CONTAINER_CRATE", "DRAW|CRATE");
            allDictionary.Add("CONTAINER_LOCKER_LARGE", "");
            allDictionary.Add("CONTAINER_LOCKER_SMALL", "");
            allDictionary.Add("CONTAINER_SHELF", "");
            allDictionary.Add("BED", "DRAW|SAVE");
            allDictionary.Add("BED_SHELTER", "DRAW|SAVE");
            allDictionary.Add("CLAY", "DRAW|ITEM");
            allDictionary.Add("CLOTH", "DRAW|ITEM");
            allDictionary.Add("DRIFTWOOD_STICK", "DRAW|ITEM");
            allDictionary.Add("ENGINE", "DRAW|ITEM");
            allDictionary.Add("ENGINE_FUEL_TANK", "DRAW|ITEM");
            allDictionary.Add("ENGINE_PROPELLER", "DRAW|ITEM");
            allDictionary.Add("ENGINE_PUMP", "DRAW|ITEM");
            allDictionary.Add("FIRE_TORCH", "DRAW|ITEM");
            allDictionary.Add("KINDLING", "DRAW|ITEM");
            allDictionary.Add("LEATHER", "DRAW|ITEM");
            allDictionary.Add("LEAVES_FIBROUS", "DRAW|ITEM");
            allDictionary.Add("PALM_FROND", "DRAW|ITEM");
            allDictionary.Add("RAWHIDE", "DRAW|ITEM");
            allDictionary.Add("ROCK", "DRAW|ITEM");
            allDictionary.Add("ROPE_COIL", "DRAW|ITEM");
            allDictionary.Add("SPYGLASS", "DRAW|ITEM");
            allDictionary.Add("STICK", "DRAW|ITEM");
            allDictionary.Add("STONE_TOOL", "DRAW|TOOL");
            allDictionary.Add("EGG_DEADEX", "DRAW|ITEM");
            allDictionary.Add("EGG_WOLLIE", "DRAW|ITEM");
            allDictionary.Add("FARMING_HOE", "DRAW|TOOL");

            allDictionary.Add("LOOM", "DRAW|INDUSTRY");
            allDictionary.Add("TANNING_RACK", "DRAW|INDUSTRY");
            allDictionary.Add("WATER_STILL", "DRAW|INDUSTRY|WATER");
            allDictionary.Add("NEW_CAMPFIRE", "DRAW|INDUSTRY|FIRE");
            allDictionary.Add("NEW_CAMPFIRE_PIT", "DRAW|INDUSTRY|FIRE");
            allDictionary.Add("NEW_CAMPFIRE_SPIT", "DRAW|INDUSTRY|FIRE");
            allDictionary.Add("FUEL_STILL", "DRAW|INDUSTRY");
            allDictionary.Add("FARMING_PLOT_CORRUGATED", "DRAW|INDUSTRY");
            allDictionary.Add("FARMING_PLOT_PLANK", "DRAW|INDUSTRY");
            allDictionary.Add("FARMING_PLOT_WOOD", "DRAW|INDUSTRY");
            allDictionary.Add("FURNACE", "DRAW|INDUSTRY");
            allDictionary.Add("HOBO_STOVE", "DRAW|INDUSTRY|FIRE");
            allDictionary.Add("SMOKER", "DRAW|INDUSTRY|FIRE");
            allDictionary.Add("BRICK_STATION", "DRAW|INDUSTRY");
            allDictionary.Add("PLANK_STATION", "DRAW|INDUSTRY");
            allDictionary.Add("TRAP_FISH", "DRAW|INDUSTRY");
            allDictionary.Add("RAIN_CATCHER", "DRAW|INDUSTRY|WATER");

            allDictionary.Add("KURA_FRUIT", "DRAW|FRUIT");
            allDictionary.Add("KURA_TREE", "DRAW|TREE");
            allDictionary.Add("QUWAWA_FRUIT", "DRAW|FRUIT");
            allDictionary.Add("QUWAWA_TREE", "DRAW|TREE");
            allDictionary.Add("CAN_BEANS", "DRAW|ITEM");
            allDictionary.Add("CAN_BEANS_OPEN", "DRAW|ITEM");
            allDictionary.Add("COCONUT_DRINKABLE", "DRAW|ITEM");
            allDictionary.Add("COCONUT_HALF", "DRAW|ITEM");
            allDictionary.Add("COCONUT_ORANGE", "DRAW|COCONUT");
            allDictionary.Add("MEAT_LARGE", "DRAW|ITEM");
            allDictionary.Add("MEAT_MEDIUM", "DRAW|ITEM");
            allDictionary.Add("MEAT_SMALL", "DRAW|ITEM");
            allDictionary.Add("POTATO", "DRAW|POTATO");
            allDictionary.Add("WATER_BOTTLE", "DRAW|ITEM");
            allDictionary.Add("WATER_SKIN", "DRAW|ITEM");
            allDictionary.Add("CORRUGATED_SHELF", "");
            allDictionary.Add("CORRUGATED_TABLE", "");
            allDictionary.Add("PLANK_CHAIR", "");
            allDictionary.Add("PLANK_SHELF", "");
            allDictionary.Add("PLANK_TABLE", "");
            allDictionary.Add("WOOD_CHAIR", "");
            allDictionary.Add("WOOD_HOOK", "");
            allDictionary.Add("WOOD_SHELF", "");
            allDictionary.Add("WOOD_TABLE", "");
            allDictionary.Add("LIGHT_HOOK", "");

            allDictionary.Add("ANTIBIOTICS", "DRAW|ITEM");
            allDictionary.Add("BANDAGE", "DRAW|ITEM");
            allDictionary.Add("MEDICAL_ALOE_SALVE", "DRAW|ITEM");
            allDictionary.Add("MORPHINE", "DRAW|ITEM");
            allDictionary.Add("NEW_COCONUT_FLASK", "DRAW|ITEM");
            allDictionary.Add("NEW_COCONUT_MEDICAL", "DRAW|ITEM");
            allDictionary.Add("VITAMINS", "DRAW|ITEM");
            allDictionary.Add("MINING_ROCK", "DRAW|RESOURCE");
            allDictionary.Add("MINING_ROCK_CLAY", "DRAW|RESOURCE");
            allDictionary.Add("ALOCASIA_1", "DRAW|TREE");
            allDictionary.Add("ALOCASIA_2", "DRAW|TREE");
            allDictionary.Add("BANANA_PLANT", "DRAW|TREE");
            allDictionary.Add("BIGROCK_1", "DRAW|ROCK");
            allDictionary.Add("BIGROCK_2", "DRAW|ROCK");
            allDictionary.Add("BIGROCK_3", "DRAW|ROCK");
            allDictionary.Add("BIGROCK_4", "DRAW|ROCK");
            allDictionary.Add("BIGROCK_5", "DRAW|ROCK");
            allDictionary.Add("CERIMAN_1", "DRAW|TREE");
            allDictionary.Add("CERIMAN_2", "DRAW|TREE");
            allDictionary.Add("CERIMAN_3", "DRAW|TREE");
            allDictionary.Add("CLIFF_001", "DRAW|ROCK|CLIFF");
            allDictionary.Add("CLIFF_002", "DRAW|ROCK|CLIFF");
            allDictionary.Add("CLIFF_003", "DRAW|ROCK|CLIFF");
            allDictionary.Add("CLIFF_004", "DRAW|ROCK|CLIFF");
            allDictionary.Add("CLIFF_005", "DRAW|ROCK|CLIFF");
            allDictionary.Add("CLIFF_006", "DRAW|ROCK|CLIFF");
            allDictionary.Add("DRIFTWOOD_DECAL", "");
            allDictionary.Add("OCEAN_BUOY", "");
            allDictionary.Add("PHILODENDRON_1", "DRAW|TREE");
            allDictionary.Add("PHILODENDRON_2", "DRAW|TREE");
            allDictionary.Add("SHORELINE_ROCK_1", "DRAW|ROCK");
            allDictionary.Add("SHORELINE_ROCK_2", "DRAW|ROCK");
            allDictionary.Add("SMALLROCK_1", "DRAW|ROCK");
            allDictionary.Add("SMALLROCK_2", "DRAW|ROCK");
            allDictionary.Add("SMALLROCK_3", "DRAW|ROCK");
            allDictionary.Add("SeaFort_1", "DRAW|SEAFORT");
            allDictionary.Add("SeaFort_2", "DRAW|SEAFORT");
            allDictionary.Add("SeaFort_3", "DRAW|SEAFORT");
            allDictionary.Add("SeaFort_Brige", "");
            allDictionary.Add("SeaFort_Brige_Broken", "");
            allDictionary.Add("DOOR", "");
            allDictionary.Add("DOOR_13M_165d", "");
            allDictionary.Add("DOOR_13M_85d", "");
            allDictionary.Add("DOOR_13_85d", "");
            allDictionary.Add("DOOR_13_165d", "");
            allDictionary.Add("DOOR_13D1", "");
            allDictionary.Add("DOOR_13D2", "");
            allDictionary.Add("DOOR_13D3", "");
            allDictionary.Add("ROWBOAT_3", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_2A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_3A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_4A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_5A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_6A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_7A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_8A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_9A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_10A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_11A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_12A", "DRAW|WRECK");
            allDictionary.Add("SHIPWRECK_13A", "DRAW|WRECK");
            allDictionary.Add("PLANEWRECK_1A", "DRAW|WRECK");
            allDictionary.Add("COMPASS", "DRAW|ITEM");
            allDictionary.Add("DUCTTAPE", "DRAW|ITEM");
            allDictionary.Add("FLARE_GUN", "DRAW|ITEM");
            allDictionary.Add("FUELCAN", "DRAW|ITEM");
            allDictionary.Add("LABEL_MAKER", "DRAW|ITEM");
            allDictionary.Add("LANTERN", "DRAW|ITEM");
            allDictionary.Add("MACHETTE", "DRAW|ITEM");
            allDictionary.Add("NEW_AIRTANK", "DRAW|ITEM");
            allDictionary.Add("BOBBER", "DRAW|ITEM");
            allDictionary.Add("ITEM_PILE", "DRAW|ITEM");

            allDictionary.Add("NEW_ARROW", "DRAW|TOOL");
            allDictionary.Add("NEW_CRUDE_AXE", "DRAW|TOOL");
            allDictionary.Add("NEW_CRUDE_BOW", "DRAW|TOOL");
            allDictionary.Add("NEW_CRUDE_HAMMER", "DRAW|TOOL");
            allDictionary.Add("NEW_CRUDE_SPEAR", "DRAW|TOOL");
            allDictionary.Add("NEW_FISHING_SPEAR", "DRAW|TOOL");
            allDictionary.Add("NEW_REFINED_AXE", "DRAW|TOOL");
            allDictionary.Add("NEW_REFINED_HAMMER", "DRAW|TOOL");
            allDictionary.Add("NEW_REFINED_KNIFE", "DRAW|TOOL");
            allDictionary.Add("NEW_REFINED_PICK", "DRAW|TOOL");
            allDictionary.Add("NEW_REFINED_SPEAR", "DRAW|TOOL");
            allDictionary.Add("NEW_SPEARGUN", "DRAW|TOOL");
            allDictionary.Add("NEW_SPEARGUN_ARROW", "DRAW|TOOL");
            allDictionary.Add("TORCH", "DRAW|TOOL");
            allDictionary.Add("FISHING_ROD", "DRAW|TOOL");

            allDictionary.Add("COCA_BUSH", "DRAW|TREE");
            allDictionary.Add("DRIFTWOOD_PILE", "");
            allDictionary.Add("FICUS_1", "DRAW|TREE|MINEABLE");
            allDictionary.Add("FICUS_2", "DRAW|TREE|MINEABLE");
            allDictionary.Add("FICUS_3", "DRAW|TREE|MINEABLE");
            allDictionary.Add("FICUS_TREE", "DRAW|TREE");
            allDictionary.Add("FICUS_TREE_2", "DRAW|TREE");
            allDictionary.Add("LOG_0", "DRAW|ITEM");
            allDictionary.Add("LOG_1", "DRAW|ITEM");
            allDictionary.Add("LOG_2", "DRAW|ITEM");
            allDictionary.Add("PALM_1", "DRAW|PALM");
            allDictionary.Add("PALM_2", "DRAW|PALM");
            allDictionary.Add("PALM_3", "DRAW|PALM");
            allDictionary.Add("PALM_4", "DRAW|PALM");
            allDictionary.Add("PALM_LOG_1", "DRAW|ITEM");
            allDictionary.Add("PALM_LOG_2", "DRAW|ITEM");
            allDictionary.Add("PALM_LOG_3", "DRAW|ITEM");
            allDictionary.Add("PALM_TOP", "DRAW|ITEM");
            allDictionary.Add("PINE_1", "DRAW|TREE");
            allDictionary.Add("PINE_2", "DRAW|TREE");
            allDictionary.Add("PINE_3", "DRAW|TREE");
            allDictionary.Add("PINE_SMALL_1", "DRAW|TREE|MINEABLE");
            allDictionary.Add("PINE_SMALL_2", "DRAW|TREE|MINEABLE");
            allDictionary.Add("PINE_SMALL_3", "DRAW|TREE|MINEABLE");
            allDictionary.Add("POTATO_PLANT", "POTATO");
            allDictionary.Add("YOUNG_PALM_1", "DRAW|RESOURCE");
            allDictionary.Add("YOUNG_PALM_2", "DRAW|RESOURCE");
            allDictionary.Add("YUCCA", "DRAW|YUCCA");
            allDictionary.Add("YUCCA_CUTTING", "DRAW|YUCCA");
            allDictionary.Add("YUCCA_HARVEST", "DRAW|YUCCA");
            allDictionary.Add("ALOE_VERA_FRUIT", "DRAW|FLOWER");
            allDictionary.Add("ALOE_VERA_PLANT", "DRAW|PLANT");
            allDictionary.Add("AJUGA_PLANT", "TREE");
            allDictionary.Add("AJUGA", "DRAW|FLOWER");
            allDictionary.Add("WAVULAVULA_PLANT", "DRAW|PLANT");
            allDictionary.Add("WAVULAVULA", "DRAW|FLOWER");
            allDictionary.Add("PIPI_PLANT", "DRAW|PLANT");
            allDictionary.Add("PIPI", "DRAW|FLOWER");
            allDictionary.Add("COCONUT_FLASK", "DRAW|ITEM");

            allDictionary.Add("MEDICAL_BREATH_BOOST", "DRAW|ITEM");
            allDictionary.Add("MEDICAL_GAUZE", "DRAW|ITEM");
            allDictionary.Add("MEDICAL_ANTIDOTE", "DRAW|ITEM");

            allDictionary.Add("PADDLE", "DRAW|TOOL");

            allDictionary.Add("PART_ELECTRICAL", "DRAW|ITEM");
            allDictionary.Add("PART_FILTER", "DRAW|ITEM");
            allDictionary.Add("PART_GYRO", "DRAW|ITEM");
            allDictionary.Add("PART_FUEL", "DRAW|ITEM");
            allDictionary.Add("PART_ENGINE", "DRAW|ITEM");
            allDictionary.Add("VEHICLE_RUDDER", "");
            allDictionary.Add("RAFT_ANCHOR", "");

            //allDictionary.Add("STRUCTURE", "DRAW|STRUCTURE");

            allDictionary.Add("MISSION_EEL", "DRAW|MISSION_EEL");
            allDictionary.Add("MISSION_MARKER", "");
            allDictionary.Add("MISSION_SHARK", "DRAW|MISSION_MEG");
            allDictionary.Add("MISSION_SQUID", "DRAW|MISSION_SQUID");
            allDictionary.Add("MISSION_TROPHY_EEL", "");
            allDictionary.Add("MISSION_TROPHY_SHARK", "");
            allDictionary.Add("MISSION_TROPHY_SQUID", "");
            allDictionary.Add("AIRCRAFT", "DRAW|MISSION_CARRIER");

            allDictionary.Add("Shelter01", "DRAW|SHELTER");
            allDictionary.Add("Shelter02", "DRAW|SHELTER");
            allDictionary.Add("Shelter03", "DRAW|SHELTER");
            allDictionary.Add("Shelter04", "DRAW|SHELTER");
            allDictionary.Add("Shelter05", "DRAW|SHELTER");
            allDictionary.Add("Shelter06", "DRAW|SHELTER");
            allDictionary.Add("Shelter07", "DRAW|SHELTER");
            allDictionary.Add("Shelter08", "DRAW|SHELTER");
            allDictionary.Add("Shelter09", "DRAW|SHELTER");

            // probable bug, there are items without keys
            allDictionary.Add("", "");


            namesDictionary = new Dictionary<string, string>();

            namesDictionary.Add("MAP_NAME_PREFIX_1", "LITTLE");
            namesDictionary.Add("MAP_NAME_PREFIX_2", "GREAT");
            namesDictionary.Add("MAP_NAME_PREFIX_3", "UPPER");
            namesDictionary.Add("MAP_NAME_PREFIX_4", "LOWER");
            namesDictionary.Add("MAP_NAME_PREFIX_5", "ENDLESS");
            namesDictionary.Add("MAP_NAME_PREFIX_6", "SOUTHERN");
            namesDictionary.Add("MAP_NAME_PREFIX_7", "WESTERN");
            namesDictionary.Add("MAP_NAME_PREFIX_8", "TAINTED");

            namesDictionary.Add("MAP_NAME_STEM_1", "BOTTOM");
            namesDictionary.Add("MAP_NAME_STEM_2", "RAGING");
            namesDictionary.Add("MAP_NAME_STEM_3", "END");
            namesDictionary.Add("MAP_NAME_STEM_4", "DARK");
            namesDictionary.Add("MAP_NAME_STEM_5", "SECRET");
            namesDictionary.Add("MAP_NAME_STEM_6", "BOTTOMLESS");
            namesDictionary.Add("MAP_NAME_STEM_7", "ENDLESS");
            namesDictionary.Add("MAP_NAME_STEM_8", "FORGOTTEN");
            namesDictionary.Add("MAP_NAME_STEM_9", "ANCIENT");
            namesDictionary.Add("MAP_NAME_STEM_10", "TROPICAL");

            namesDictionary.Add("MAP_NAME_SUFFIX_1", "ISLAND");
            namesDictionary.Add("MAP_NAME_SUFFIX_2", "ATOLL");
            namesDictionary.Add("MAP_NAME_SUFFIX_3", "CREST");
            namesDictionary.Add("MAP_NAME_SUFFIX_4", "CORNER");
            namesDictionary.Add("MAP_NAME_SUFFIX_5", "SANCTUARY");
            namesDictionary.Add("MAP_NAME_SUFFIX_6", "ESCAPE");
        }
    }
}
