using Raylib_cs;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace HnTRgine
{
    class Game
    {
        public static Scene CurrentScene { get; private set; } = new Scene();
        public static Vector2I screenSize { get => new Vector2I(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()); }

        public Game(string? initalScenePath=null)
        {
            Raylib.InitWindow(1280, 720, "TheGame");
            if (initalScenePath is not null)
                CurrentScene = Scene.LoadScene(initalScenePath);
        }

        public void GameLoop()
        {
            Init();
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Update();    
                Raylib.DrawFPS(0, 0);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }

        public void Init()
        {
            CurrentScene.InitSystems();
        }

        public void Update()
        {
            CurrentScene.UpdateSystems();
        }
    }

    class Entity
    {
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        public int Id;
        public string name;
        public Transform transform { get; private set; }

        public Entity(string name="")
        {
            this.name = name;
            transform = AddComponent(new Transform());
        }

        public bool HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }

        public bool HasComponent(Type component)
        {
            return components.Keys.Any(key => component.IsAssignableFrom(key));
        }

        public T AddComponent<T>(T component) where T : Component
        {
            if (HasComponent(component.GetType()))
                throw new Exception($"Can't add component! Entity {this} already have {component.GetType()} component");
            component.owner = this;
            components[component.GetType()] = component;
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            if (!components.TryGetValue(typeof(T), out Component? t))
                throw new Exception($"Can't get component! Entity {this} do not have {typeof(T)} component");
            return (T)t;
        }

        public void RemoveComponent<T>() where T : Component
        {
            if (!HasComponent<T>())
                throw new Exception($"Can't remove component! Entity {this} do not have {typeof(T)} component");
            components.Remove(typeof(T));
        }
        
        public IEnumerable<Script> GetScripts()
        {
            return components.Where(kvp => typeof(Script).IsAssignableFrom(kvp.Key)).Select(kvp => kvp.Value as Script);
        }

        public override string ToString()
        {
            return $"Entity(name: {name}, id: {Id})";
        }

        public void Destroy()
        {
            Game.CurrentScene.RemoveEntity(Id);
        }

        public JsonArray GetComponentsJson()
        {
            JsonArray jsonArray = new JsonArray();
            foreach (Component component in components.Values)
            {
                JsonObject c = new JsonObject();
                c["type"] = component.GetType().ToString();
                component.SaveToJson(ref c);
                jsonArray.Add(c);
            }
            return jsonArray;
        }
    }

    class Scene
    {
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        private Dictionary<Type, System> systems = new Dictionary<Type, System>();
        private int IdCounter = 0;

        public Scene() {}

        public int GetId() => IdCounter++;

        public Entity AddEntity(Entity entity, int? id=null)
        {
            if (id is null)
                id = GetId();
            entity.Id = (int)id;
            entities[(int)id] = entity;
            foreach (System s in systems.Values)
            {
                bool haveComponents = true;
                foreach (Type requiredComponent in s.RequiredComponents)
                {
                    if (!entity.HasComponent(requiredComponent))
                    {
                        haveComponents = false;
                        break;
                    }
                }
                if (haveComponents)
                    s.Init(entity);
            }
            return entity;
        }

        public Entity GetEntity(int id)
        {
            if (!entities.TryGetValue(id, out Entity? e))
                throw new Exception($"Can't get entity! Scene do not have entity with id: {id}");
            return e;
        }

        public Entity GetEntity(string name)
        {
            foreach (Entity e in entities.Values)
                if (e.name == name) return e;
            throw new Exception($"Can't get entity! Scene do not have entity with name: {name}");
        }

        public void RemoveEntity(int id)
        {
            if (!entities.ContainsKey(id))
                throw new Exception($"Can't remove entity! Scene do not have entity with id: {id}");
            entities.Remove(id);
        }

        public void RemoveEntity(string name)
        {
            foreach (Entity e in entities.Values)
                if (e.name == name) entities.Remove(e.Id);
            throw new Exception($"Can't remove entity! Scene do not have entity with name: {name}");
        }
        
        public bool HasSystem<T>() where T : System
        {
            return HasSystem(typeof(T));
        }

        public bool HasSystem(Type system)
        {
            return systems.ContainsKey(system.GetType());
        }

        public T AddSystem<T>(T system) where T : System
        {
            if (HasSystem(system.GetType()))
                throw new Exception($"Can't add system! Scene already have {system.GetType()} system!");
            systems[system.GetType()] = system;
            foreach (Entity e in entities.Values)
            {
                bool haveComponents = true;
                foreach (Type requiredComponent in system.RequiredComponents)
                {
                    if (!e.HasComponent(requiredComponent))
                    {
                        haveComponents = false;
                        break;
                    }
                }
                if (haveComponents)
                    system.Init(e);
            }
            return system;
        }

        public void RemoveSystem<T>() where T : System
        {
            if (!HasSystem<T>())
                throw new Exception($"Can't remove system! Scene do not have {typeof(T)} system!");
            systems.Remove(typeof(T));
        }

        public void InitSystems()
        {
            foreach (System s in systems.Values)
            {
                foreach (Entity e in entities.Values)
                {
                    bool haveComponents = true;
                    foreach (Type requiredComponent in s.RequiredComponents)
                    {
                        if (!e.HasComponent(requiredComponent))
                        {
                            haveComponents = false;
                            break;
                        }
                    }
                    if (haveComponents)
                        s.Init(e);
                }
            }
        }

        public void UpdateSystems()
        {
            foreach (System s in systems.Values)
            {
                foreach (Entity e in entities.Values)
                {
                    bool haveComponents = true;
                    foreach (Type requiredComponent in s.RequiredComponents)
                    {
                        if (!e.HasComponent(requiredComponent))
                        {
                            haveComponents = false;
                            break;
                        }
                    }
                    if (haveComponents)
                        s.Update(e);
                }
            }
        }

        public static Scene LoadScene(string scenePath)
        {
            Scene scene = new Scene();

            string sceneFile = File.ReadAllText(Path.GetFullPath(scenePath));
            using JsonDocument doc = JsonDocument.Parse(sceneFile);
            JsonElement root = doc.RootElement;

            foreach (JsonElement element in root.GetProperty("Entities").EnumerateArray())
            {
                Entity e = new Entity();
                e.Id = element.GetProperty("id").GetInt32();
                e.name = element.GetProperty("name").ToString();

                foreach (JsonElement componentElement in element.GetProperty("components").EnumerateArray())
                {
                    string? typeName = componentElement.GetProperty("type").GetString();
                    if (string.IsNullOrEmpty(typeName))
                        continue;

                    Type? componentType = Type.GetType(typeName);
                    if (componentType == null)
                    {
                        Console.WriteLine($"Warning: Could not find type '{typeName}' for script component. Skipping.");
                        continue;
                    }

                    object? instance = Activator.CreateInstance(componentType);
                    if (instance is Component component)
                    {
                        if (component is not Transform)
                        {
                            component.LoadFromJson(componentElement);
                            e.AddComponent(component);
                        }
                        else
                            e.transform.LoadFromJson(componentElement);
                    }
                    else
                        throw new Exception($"Failed to create component instance of type '{typeName}'.");
                }

                scene.AddEntity(e, e.Id);
            }

            foreach (JsonElement element in root.GetProperty("Systems").EnumerateArray())
            {
                string? systemName = element.GetString();
                if (string.IsNullOrEmpty(systemName))
                    continue;

                Type? systemType = Type.GetType(systemName);
                if (systemType == null)
                {
                    Console.WriteLine($"Warning: Could not find type '{systemName}' for script component. Skipping.");
                    continue;
                }

                object? instance = Activator.CreateInstance(systemType);
                if (instance is System system)
                    scene.AddSystem(system);
            }

            return scene;
        }

        public void SaveScene(string path)
        {
            JsonObject root = new JsonObject();

            JsonArray entitiesJson = new JsonArray();
            foreach (Entity e in entities.Values)
            {
                JsonObject eJson = new JsonObject();
                eJson["id"] = e.Id;
                eJson["name"] = e.name;

                JsonArray components = e.GetComponentsJson();
                eJson.Add("components", components);

                entitiesJson.Add(eJson);
            }
            root.Add("Entities", entitiesJson);

            JsonArray systemsJson = new JsonArray();
            foreach (System s in systems.Values)
            {
                systemsJson.Add(s.GetType().ToString());
            }
            root.Add("Systems", systemsJson);

            File.WriteAllText(Path.GetFullPath(path), root.ToString());
        }
    }
}
