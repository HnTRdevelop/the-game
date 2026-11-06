using System.Text.Json;
using System.Text.Json.Nodes;


namespace HnTRgine.SceneManagement
{
    using HnTRgine.Components;
    using HnTRgine.Systems;
    using HnTRgine.EntityManagement;

    class Scene
    {
        private Dictionary<ulong, Entity> entities = new Dictionary<ulong, Entity>();
        private Dictionary<Type, System> systems = new Dictionary<Type, System>();
        private ulong IdCounter = 0;

        public Scene() {}

        public ulong GetId()
        {
            while (entities.ContainsKey(IdCounter)) IdCounter++;
            return IdCounter;
        }

        public Entity AddEntity(Entity entity, ulong? id=null)
        {
            if (id is null)
                id = GetId();
            entity.Id = (ulong)id;
            entities[(ulong)id] = entity;
            if (Game.IsRunning)
                foreach (System s in systems.Values.ToArray())
                    InitSystem(s, entity);
            return entity;
        }

        public Entity GetEntity(ulong id)
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

        public void RemoveEntity(ulong id)
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
            return systems.ContainsKey(system);
        }

        public T AddSystem<T>(T system) where T : System
        {
            if (HasSystem(system.GetType()))
                throw new Exception($"Can't add system! Scene already have {system.GetType()} system!");
            systems[system.GetType()] = system;
            if (Game.IsRunning)
                foreach (Entity e in entities.Values.ToArray())
                    InitSystem(system, e);
            return system;
        }

        public void RemoveSystem<T>() where T : System
        {
            if (!HasSystem<T>())
                throw new Exception($"Can't remove system! Scene do not have {typeof(T)} system!");
            systems.Remove(typeof(T));
        }

        public T GetSystem<T>() where T : System
        {
            if (!systems.TryGetValue(typeof(T), out System? s))
                throw new Exception($"Can't get system! Scene do not have {typeof(T)} system!");
            return (T)s;
        }

        public void InitSystem(System system, Entity entity)
        {
            bool haveComponents = true;
            foreach (Type requiredComponent in system.RequiredComponents)
            {
                if (!entity.HasComponent(requiredComponent))
                {
                    haveComponents = false;
                    break;
                }
            }
            if (haveComponents)
                system.Init(entity);
        }

        public void UpdateSystem(System system, Entity entity)
        {
            bool haveComponents = true;
            foreach (Type requiredComponent in system.RequiredComponents)
            {
                if (!entity.HasComponent(requiredComponent))
                {
                    haveComponents = false;
                    break;
                }
            }
            if (haveComponents)
                system.Update(entity);
        }

        public void InitSystems()
        {
            foreach (System s in systems.Values.ToArray())
                foreach (Entity e in entities.Values.ToArray())
                    InitSystem(s, e);
        }

        public void UpdateSystems()
        {
            foreach (System s in systems.Values.ToArray())
                foreach (Entity e in entities.Values.ToArray())
                    UpdateSystem(s, e);
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
                e.Id = element.GetProperty("id").GetUInt64();
                e.name = element.GetProperty("name").ToString();

                foreach (JsonElement componentElement in element.GetProperty("components").EnumerateArray())
                {
                    string? componentName = componentElement.GetProperty("type").GetString();
                    if (string.IsNullOrEmpty(componentName))
                        continue;

                    Type componentType = Type.GetType(componentName) ?? throw new NullReferenceException($"Could not find type '{componentName}' for script component.");

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
                        throw new Exception($"Failed to create component instance of type '{componentName}'.");
                }

                scene.AddEntity(e, e.Id);
            }

            foreach (JsonElement element in root.GetProperty("Systems").EnumerateArray())
            {
                string? systemName = element.GetString();
                if (string.IsNullOrEmpty(systemName))
                    continue;

                Type systemType = Type.GetType(systemName) ?? throw new NullReferenceException($"Could not find type '{systemName}' for script component.");

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
