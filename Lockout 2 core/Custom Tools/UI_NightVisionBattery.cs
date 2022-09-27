using UnityEngine;

namespace Lockout_2_core.Custom_Tools
{
    class BatterySegment
    {
        public BatterySegment(string name, Transform parent, float offsetX)
        {
            gameObject = GameObject.Instantiate(GuiManager.Current.m_playerLayer.m_playerStatus.m_health1.gameObject, parent, false);
            gameObject.name = name;
            m_renderer = gameObject.GetComponent<SpriteRenderer>();
            m_renderer.size = m_size;
            m_renderer.color = m_color;
            m_renderer.sortingOrder = m_renderOrder;
            SetOffset(offsetX);
        }

        public void SetPosition(Vector2 position)
        {
            m_position = position;
            gameObject.transform.localPosition = m_position;
        }
        public void SetPosition(float x, float y)
        {
            m_position = new(x, y);
            gameObject.transform.localPosition = m_position;
        }



        public void SetOffset(float value)
        {
            gameObject.transform.localPosition = m_position;
            m_offsetX = value;
            gameObject.transform.Translate(m_offsetX, 0, 0);
        }



        public void SetSize(Vector2 size)
        {
            m_size = size;
            m_renderer.size = m_size;
        }
        public void SetSize(float width, float height)
        {
            m_size = new(width, height);
            m_renderer.size = m_size;
        }



        public void SetColor(Color color)
        {
            m_color = color;
            m_renderer.color = m_color;
        }
        public void SetColor(float r, float g, float b, float a = 1)
        {
            m_color = new(r, g, b, a);
            m_renderer.color = m_color;
        }



        public void SetOrder(int order)
        {
            m_renderOrder = order;
            m_renderer.sortingOrder = m_renderOrder;
        }



        public void SetActive(bool enabled)
        {
            m_active = enabled;
            gameObject.active = m_active;
        }
        public void Toggle()
        {
            m_active = !gameObject.active;
            gameObject.active = m_active;
        }



        public void Set(BatteryBackground settings)
        {
            SetPosition(settings.m_position);
            SetSize(settings.m_size);
            SetColor(settings.m_color);
            SetOrder(settings.m_renderOrder);
        }

        bool m_active;
        float m_offsetX;

        Vector3 m_position = new();
        Vector2 m_size = new(15f, 35f);
        Color m_color = BatteryBar.BaseColor;
        int m_renderOrder = 2;

        public GameObject gameObject;
        SpriteRenderer m_renderer;
    }

    class BatteryBackground
    {
        public BatteryBackground(Vector2 position, Vector2 size, Color color, int order = 2)
        {
            m_position = position;
            m_size = size;
            m_color = color;
            m_renderOrder = order;
        }
        public Vector3 m_position = new();
        public Vector2 m_size = new(15f, 35f);
        public Color m_color = BatteryBar.BaseColor;
        public int m_renderOrder;
    }

    class BatteryBar
    {
        public BatteryBar(int length, Transform parent)
        {
            gameObject = new("BatteryBar");
            gameObject.transform.SetParent(parent);
            SetPosition(m_position);
            segments = new BatterySegment[length];
            for (int i = 0; i < length; ++i)
            {
                segments[i] = new($"Segment {i}", gameObject.transform, i * m_spacing);
            }

            background = new BatterySegment[7];
            for (int i = 0; i < 7; i++)
            {
                background[i] = new($"BackgroundPiece {i}", gameObject.transform, 0f);
                background[i].Set(backgroundSettings[i]);
            }


            SetScale(m_scale);
            SetActive(false);
        }

        public void SetSpacing(float value)
        {
            m_spacing = value;
            for (int i = 0; i < segments.Length; ++i)
            {
                segments[i].SetOffset(i * m_spacing);
            }
        }
        public void SetPosition(Vector3 position)
        {
            m_position = position;
            gameObject.transform.localPosition = m_position;
        }
        public void SetPosition(float x, float y, float z)
        {
            m_position = new(x, y, z);
            gameObject.transform.localPosition = m_position;
        }
        public void SetScale(Vector3 scale)
        {
            m_scale = scale;
            gameObject.transform.localScale = m_scale;
        }
        public void SetActive(bool enabled)
        {
            m_active = enabled;
            gameObject.active = m_active;
        }
        public void UpdateSegments(float ammo, float ammoMaxCap)
        {
            var count = Mathf.Ceil((ammo / ammoMaxCap) * segments.Length);
            for (var i = 0; i < segments.Length; i++)
            {
                segments[i].SetActive(i < count);
            }
        }




        Vector3 m_position = new(380, -26, 0);
        Vector3 m_scale = Vector3.one * 0.75f;
        float m_spacing = 20f;
        bool m_active;
        public GameObject gameObject;
        BatterySegment[] segments;
        BatterySegment[] background;
        BatteryBackground[] backgroundSettings = new BatteryBackground[]
        {
            new(new(-9,0), new(113,48), BGColor, 1),
            new(new(105,0), new(4,35), BGColor, 1),

            new(new(-7,0), new(2,42), BaseColor),
            new(new(100,0), new(2,42), BaseColor),
            new(new(-7,22), new(109,2), BaseColor),
            new(new(-7,-22), new(109,2), BaseColor),
            new(new(102,0), new(5,30), BaseColor)
        };

        public static Color BaseColor = new(0.9f, 0.7f, 0.3f, 1.0f);
        public static Color BGColor = new(0.3f, 0.1f, 0.5f, 0.05f);
    }
}