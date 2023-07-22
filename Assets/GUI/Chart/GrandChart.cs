﻿using Conquest.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Conquest.Assets.GUI.Chart
{
    public class GrandChart : UIState
    {
        private UIElement etElmt;
        private UIImage etImg;
        public static Asset<Texture2D> PlayDesert;
        public static bool GSB = false;
        public static UIText txt1 = new UIText("", 1, false);
        public static UIImage infoImg = new UIImage(ModContent.Request<Texture2D>("Conquest/Assets/GUI/Textbox"));
        private void SetRectangle(UIElement uiElmt, float left, float top, float width, float height)
        {
            uiElmt.Left.Set(left, 0f);
            uiElmt.Top.Set(top, 0f);
            uiElmt.Width.Set(width, 0f);
            uiElmt.Height.Set(height, 0f);
        }
        private Vector2 offset;
        private bool dragging;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            DragStart(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 endMousePosition = evt.MousePosition;
            dragging = false;

            Left.Set(endMousePosition.X - offset.X, 0f);
            Top.Set(endMousePosition.Y - offset.Y, 0f);

            Recalculate();
        }
        public override void OnInitialize()
        {
            etElmt = new UIElement();

            SetRectangle(etElmt, left: 320f, top: 0f, width: 980f, height: 960f);


            etImg = new UIImage(ModContent.Request<Texture2D>("Conquest/Assets/GUI/Chart/Chart"));

            SetRectangle(etImg, left: 0, top: 0f, width: 980f, height: 960f);

            Append(etElmt);
            etElmt.Append(etImg);

            Asset<Texture2D> etButton0 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Chart/ChartX");

            ETButton playButton0 = new ETButton(etButton0, "Button0");

            SetRectangle(playButton0, left: 79f, top: 77f, width: 84f, height: 84f);

            playButton0.OnLeftClick += new MouseEvent(EtButton0_OnClick);

            etElmt.Append(playButton0);

            Asset<Texture2D> etButton2 = ModContent.Request<Texture2D>("Conquest/NPCs/Bosses/Anubis/Anubis_Head_Boss");

            ETButton playButton2 = new ETButton(etButton2, "Button2");

            SetRectangle(playButton2, left: 755f, top: 537f, width: 22f, height: 26f);

            playButton2.OnLeftClick += new MouseEvent(EtButton2_OnClick);
            playButton2.OnMouseOver += new MouseEvent(Desert);
            playButton2.OnMouseOut += new MouseEvent(Desert2);
            etElmt.Append(playButton2);


            Asset<Texture2D> etButton3 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Chart/DandukeIcon");

            ETButton playButton3 = new ETButton(etButton3, "Button3");

            SetRectangle(playButton3, left: 225f, top: 245f, width: 28f, height: 24f);
            playButton3.OnMouseOver += new MouseEvent(FlowerField);
            playButton3.OnMouseOut += new MouseEvent(FlowerField2);
            playButton3.OnLeftClick += new MouseEvent(FlowerFieldOnClick);
            etElmt.Append(playButton3);


            Asset<Texture2D> etButton4 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Chart/RatkingIcon");

            ETButton playButton4 = new ETButton(etButton4, "Button4");

            SetRectangle(playButton4, left: 428f, top: 855f, width: 28f, height: 24f);


            etElmt.Append(playButton4);

            Asset<Texture2D> etButton5 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Chart/Pirate");

            ETButton playButton5 = new ETButton(etButton5, "Button5");

            SetRectangle(playButton5, left: 355f, top: 460f, width: 46f, height: 42f);


            etElmt.Append(playButton5);


        }
        private void FlowerField(UIMouseEvent evt, UIElement listeningElement)
        {
            SetRectangle(GrandChart.infoImg, left: 680f, top: 245, width: 160f, height: 240f);
            if (NPC.downedBoss3)
            {
                txt1.SetText("\n          Flower Field\n\n       Left click to enter");
            }
            else if (!NPC.downedPlantBoss)
            {
                txt1.SetText("\n          Flower Field\n\n   Requirments: Slay Skeletron!");
            }
            Append(infoImg);
            GrandChart.infoImg.Append(txt1);
        }
        private void FlowerField2(UIMouseEvent evt, UIElement listeningElement)
        {
            infoImg.Remove();
        }
        private void FlowerFieldOnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (NPC.downedBoss3 && GSB == true)
            {
                if (SubworldSystem.Current != null)
                {
                    SubworldSystem.Exit();
                }
                else
                {
                    SubworldSystem.Enter<FlowerField>();
                }
                infoImg.Remove();
                GSB = false;
            }
        }
        private void Desert(UIMouseEvent evt, UIElement listeningElement)
        {
            SetRectangle(GrandChart.infoImg, left: 700f, top: 530f, width: 160f, height: 240f);
            if (NPC.downedPlantBoss)
            {
                txt1.SetText("\n          Desert Temple\n\n       Left click to enter");
            }
            else if(!NPC.downedPlantBoss)
            {
                txt1.SetText("\n          Desert Temple\n\n   Requirments: Slay Plantera!");
            }
            Append(infoImg);
            GrandChart.infoImg.Append(txt1);
        }
        private void Desert2(UIMouseEvent evt, UIElement listeningElement)
        {
            infoImg.Remove();
        }
        private void EtButton0_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            infoImg.Remove();
            GSB = false;
        }

        private void EtButton2_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (NPC.downedPlantBoss && GSB == true)
            {
                if (SubworldSystem.Current != null)
                {
                    SubworldSystem.Exit();
                }
                else
                {
                    SubworldSystem.Enter<DesertTemple>();
                }
                infoImg.Remove();
                GSB = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GSB != true)
                return;
            Main.LocalPlayer.mouseInterface = true;


            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (GSB != true)
                return;
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X and Main.mouseX are the same
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            
            if (GSB != true)
                return;
            base.Update(gameTime);
        }

    }

    internal class GTButton : SoundlessButton
    {
        internal string etHv;
        public GTButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            this.etHv = hoverText;
        }
    }
    class GrandChartUI : ModSystem
    {
        private UserInterface etItfc;

        internal GrandChart grandChart;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                grandChart = new();
                etItfc = new();
                etItfc.SetState(grandChart);
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            etItfc?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int etIdx = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars")) + 9;

            if (etIdx != -1)
            {
                layers.Insert(etIdx, new LegacyGameInterfaceLayer(
                    "Conquest: GrandChart",
                    delegate
                    {
                        etItfc.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                    )
                );
            }
        }
    }
}
