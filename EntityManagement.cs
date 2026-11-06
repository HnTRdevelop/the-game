using System.Text.Json.Nodes;


namespace HnTRgine.EntityManagement
{
    using HnTRgine.Components;
    using HnTRgine.Systems;

    class Entity
    {
        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        public ulong Id;
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
            if (component is Script && Game.IsRunning && Game.CurrentScene.HasSystem<ScriptManagementSystem>())
                Game.CurrentScene.InitSystem(Game.CurrentScene.GetSystem<ScriptManagementSystem>(), this);
            if (component is Camera && Game.mainCamera is null) Game.mainCamera = component as Camera;
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
}
