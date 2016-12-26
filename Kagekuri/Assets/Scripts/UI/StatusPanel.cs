using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kagekuri
{
    public class StatusPanel : Singleton<StatusPanel>
    {
        private StatusWindow _MainWindow;
        private StatusWindow _SubWindow;

        protected override void Awake()
        {
            base.Awake();
            _MainWindow = new StatusWindow(GameObject.Find("/Canvas/StatusPanel/MainStatusWindow"));
            _SubWindow = new StatusWindow(GameObject.Find("/Canvas/StatusPanel/SubStatusWindow"));
            _MainWindow.Hide();
            _SubWindow.Hide();
        }

        public void ShowMain(ActiveUnit target)
        {
            _MainWindow.Show(target);
        }

        public void ShowSub(ActiveUnit target)
        {
            _SubWindow.Show(target);
        }

        public void HideMain()
        {
            _MainWindow.Hide();
        }

        public void HideSub()
        {
            _SubWindow.Hide();
        }

        public void HideAll()
        {
            _MainWindow.Hide();
            _SubWindow.Hide();
        }

        private class StatusWindow
        {
            public ActiveUnit Target;
            GameObject WindowPanel;
            public Image UnitImage;
            public Text NameText;
            public Text LevelText;
            public Bar HPBar;
            public Text HPText;
            public Bar SPBar;
            public Text SPText;
            public Bar APBar;
            public Text APText;

            public StatusWindow(GameObject window)
            {
                WindowPanel = window;

                var r = WindowPanel.transform as RectTransform;
                UnitImage = r.FindChild("UnitImage").GetComponent<Image>();
                NameText = r.FindChild("NameText").GetComponent<Text>();
                LevelText = r.FindChild("LevelText").GetComponent<Text>();
                HPBar = new Bar(r.FindChild("HPBar").gameObject);
                HPText = r.FindChild("HPText").GetComponent<Text>();
                SPBar = new Bar(r.FindChild("SPBar").gameObject);
                SPText = r.FindChild("SPText").GetComponent<Text>();
                APBar = new Bar(r.FindChild("APBar").gameObject);
                APText = r.FindChild("APText").GetComponent<Text>();
            }

            public void Show(ActiveUnit target)
            {
                Target = target;
                SetParameters();
                WindowPanel.SetActive(true);
            }

            public void Hide()
            {
                WindowPanel.SetActive(false);
            }

            public void SetParameters()
            {
                var status = Target.Status;
                NameText.text = status.Name;
                LevelText.text = status.Level.ToString();
                HPBar.Set((double)status.HP / status.MaxHP);
                SPBar.Set((double)status.SP / status.MaxSP);
                APBar.Set((double)status.AP / status.MaxAP);
                HPText.text = status.HP.ToString("##0").PadLeft(3) + "/" + status.MaxHP.ToString("##0").PadLeft(3);
                SPText.text = status.SP.ToString("##0").PadLeft(3) + "/" + status.MaxSP.ToString("##0").PadLeft(3);
                APText.text = status.AP.ToString("##0").PadLeft(3) + "/" + status.MaxAP.ToString("##0").PadLeft(3);
            }

            public void Update()
            {
                SetParameters();
            }

            public class Bar
            {
                public Image Frame;
                public Image Foreground;
                private GameObject background;
                public List<Image> Backgrounds;
                public const int Width = 164;
                public Bar(GameObject bar)
                {
                    var tf = bar.transform;
                    Frame = tf.FindChild("Frame").GetComponent<Image>();
                    Foreground = tf.FindChild("Foreground").GetComponent<Image>();
                    background = tf.FindChild("Background").gameObject;
                    Backgrounds = new List<Image>();
                    for(var i = 0;i < Width;i++)
                    {
                        var bg = Instantiate(background);
                        var rtf = bg.transform as RectTransform;
                        rtf.SetParent(tf);
                        rtf.position = background.transform.position + new Vector3(-(i + 1), 0, 0);
                        Backgrounds.Add(rtf.GetComponent<Image>());
                    }
                    background.gameObject.SetActive(false);

                    Set(0.9);
                }

                public void Set(double rate)
                {
                    var count = (int)(rate * Width);
                    for(var i = 0;i < Backgrounds.Count;i++)
                    {
                        if (i < count)
                            Backgrounds[Backgrounds.Count - i - 1].gameObject.SetActive(false);
                        else
                            Backgrounds[Backgrounds.Count - i - 1].gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}