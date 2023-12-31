﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader.UI.Elements;

namespace Conquest.Assets.GUI
{
    internal class T1 : UIState
    {
        Player player = Main.LocalPlayer;
        SoundStyle Selected = new SoundStyle($"{nameof(Conquest)}/Assets/Sounds/Selected");
        private UIElement t1Elmt;
        private UIImage t1Img;

        public static Asset<Texture2D> t1p0;
        public static Asset<Texture2D> t1p1;
        public static Asset<Texture2D> t1p2;
        public static Asset<Texture2D> t1p3;
        public static Asset<Texture2D> t1p4;
        public static Asset<Texture2D> t1p5;
        public static Asset<Texture2D> t1p6;

        public static bool p1On = false;
        public static bool p2On = false;
        public static bool p3On = false;
        public static bool p4On = false;
        public static bool p5On = false;
        public static bool p6On = false;
        public static bool p7On = false;

        public static UIText txt1 = new UIText("", 1, false);
        public override void OnInitialize()
        {
            t1Elmt = new UIElement();

            SetRectangle(t1Elmt, left: 320f, top: 0f, width: 1024f, height: 768f);


            t1Img = new UIImage(ModContent.Request<Texture2D>("Conquest/Assets/GUI/T1"));

            SetRectangle(t1Img, left: 320f, top: 0f, width: 384f, height: 384f);

            Append(t1Elmt);
            t1Elmt.Append(t1Img);


        }
        private void T1P0Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 504f, top: 110f, width: 160f, height: 240f);

            if (p1On != true)
            {
                txt1.SetText("Spirit Vein：\n+2% Magic Damage\n\n\n\nLeft Click to activate\n+1 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            else
            {
                txt1.SetText("Spirit Vein：\n+2% Magic Damage\n\n\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P1Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 458f, top: 126f, width: 160f, height: 240f);

            if (p2On != true)
            {
                if(p1On == true && NPC.downedBoss1 == true)
                {
                    txt1.SetText("Scorching Spells：\nMagic Attacks inflict\nMagic Burn!\n\n\n\nLeft Click to activate\n+2 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (NPC.downedBoss1 != true)
                {
                    txt1.SetText("Scorching Spells：\nMagic Attacks inflict\nMagic Burn!\n\nSlay the Eye of Cthulu\nto unlock!\n\n+2 Load\nCurrent Load：：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (p1On != true)
                {
                    txt1.SetText("Scorching Spells：\nMagic Attacks inflict\nMagic Burn\n\nLocked！：\nYou need to unlock\nthe previous talent first\n+2 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
            }
            else
            {
                txt1.SetText("Scorching Spells：\nMagic Attacks inflict\nMagic Burn!\n\n\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }

            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P2Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 458f, top: 126f, width: 160f, height: 240f);

            if(p3On != true)
            {
                if (p1On == true && NPC.downedBoss1 == true)
                {
                    txt1.SetText("Magic Reflux：\nMagic projectiles have\na chance to restore\nmana on death\n\nLeft Click to activate\n+2 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (NPC.downedBoss1 != true)
                {
                    txt1.SetText("Magic Reflux：\nMagic projectiles have\na chance to restore\nmana on death\nSlay the Eye of Cthulu\nto unlock!：\n+2 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (p1On != true)
                {
                    txt1.SetText("Magic Reflux：\nMagic projectiles have\na chance to restore\nmana on death\nLocked！：\nYou need to unlock the\nthe previous talent first\n+2 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }

            }
            else
            {
                txt1.SetText("Magic Reflux：\nMagic projectiles have\na chance to restore\nmana on death\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P3Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 458f, top: 126f, width: 160f, height: 240f);

            if (p4On != true)
            {
                if (p2On == true && Main.hardMode == true)
                {
                    txt1.SetText("Resonance：\nIncreases magic damage\nBased on your\ncurrent mana\n\n\nLeft Click to activate\n+3 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (Main.hardMode != true)
                {
                    txt1.SetText("Resonance：\nIncreases magic damage\nBased on your\ncurrent mana\n\nLocked！：\nHard Mode!\n+3 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (p2On != true)
                {
                    txt1.SetText("Resonance：\nIncreases magic damage\nBased on your\ncurrent mana\nLocked！\nYou need to unlock the\nthe previous talent first   \n+3 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }

            }
            else
            {
                txt1.SetText("Resonance：\nIncreases magic damage\nBased on your\ncurrent mana\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P4Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 458f, top: 126f, width: 160f, height: 240f);

            if (p5On != true)
            {
                if (p3On == true && Main.hardMode == true)
                {
                    txt1.SetText("Magic Absorb：\nHostile projectiles\ngrant mana based\n on their damage\n\n\nLeft Click to activate\n+3 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (Main.hardMode != true)
                {
                    txt1.SetText("Magic Absorb：\nHostile projectiles\ngrant mana based\non their damage\n\nLocked！：\nHard Mode!\n3 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (p3On != true)
                {
                    txt1.SetText("Magic Absorb：\nHostile projectiles\ngrant mana based\non their damage\nLocked！：\nYou need to unlock the\nthe previous talent first\n+3 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }

            }
            else
            {
                txt1.SetText("Magic Absorb：\nHostile projectiles\ngrant mana based\non their damage\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P5Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 458f, top: 126f, width: 160f, height: 240f);

            if (p6On != true)
            {
                if (p4On == true && Main.hardMode == true)
                {
                    txt1.SetText("Land Mine：\nIf a magic projectile\nhits a tile\nIt will leave behind\na energy landmine\n\n\nLeft Click to activate\n+4 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (Main.hardMode != true)
                {
                    txt1.SetText("Land Mine：\nIf a magic projectile\nhits a tile\nIt will leave behind\na energy landmine\nLocked！：\nHard Mode!\n+4 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (p4On != true)
                {
                    txt1.SetText("Land Mine：\nIf a magic projectile\nhits a tile\nIt will leave behind\na energy landmine\nLocked！\nYou need to unlock the\nthe previous talent first\n+4 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }

            }
            else
            {
                txt1.SetText("Land Mine：\nIf a magic projectile\nhits a tile\nIt will leave behind\na energy landmine\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P6Info(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            SetRectangle(EightTrigrams.infoImg, left: 458f, top: 126f, width: 160f, height: 240f);

            if (p7On != true)
            {
                if (p5On == true && Main.hardMode == true)
                {
                    txt1.SetText("Star Spirit：\nGrants a Star Spirit\nwho provides random effects\nevery 15 seconds\n\n\nLeft Click to activate\n+4 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (Main.hardMode != true)
                {
                    txt1.SetText("Star Spirit：\nGrants a Star Spirit\nwho provides random\neffects every 15 seconds\nLocked!\nHard Mode!\n+4 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }
                else if (p5On != true)
                {
                    txt1.SetText("Star Spirit：\nGrants a Star Spirit\nwho provides random effects\nevery 15 seconds\nLocked!\nYou need to unlock the\nthe previous talent first\n+4 Load\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);

                }

            }
            else
            {
                txt1.SetText("Star Spirit：\nGrants a Star Spirit\nwho provides random effects\nevery 15 seconds\n\n\n\nActivated！\nCurrent Load：" + EightTrigrams.loadInfoNow + "/" + EightTrigrams.loadInfoMax);
            }
            Append(EightTrigrams.infoImg);
            EightTrigrams.infoImg.Append(txt1);
        }

        private void T1P0InfoClose(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tType != 1)
                return;

            EightTrigrams.infoImg.Remove();
        }
        private void T1P0On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 1 > EightTrigrams.tMaxLoad || p1On == true)
                return;

            p1On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 1;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void T1P1On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 2 > EightTrigrams.tMaxLoad || p2On == true || p1On == false || NPC.downedBoss1 == false)
                return;

            p2On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 2;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void T1P2On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 2 > EightTrigrams.tMaxLoad || p3On == true || p1On == false || NPC.downedBoss1 == false)
                return;

            p3On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 2;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void T1P3On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 3 > EightTrigrams.tMaxLoad || p4On == true || p2On == false || Main.hardMode == false)
                return;

            p4On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 3;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void T1P4On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 3 > EightTrigrams.tMaxLoad || p5On == true || p3On == false || Main.hardMode == false)
                return;

            p5On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 3;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void T1P5On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 4 > EightTrigrams.tMaxLoad || p6On == true || p4On == false || Main.hardMode == false)
                return;

            p6On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 4;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void T1P6On(UIMouseEvent evt, UIElement listeningElement)
        {
            if (EightTrigrams.tLoad + 4 > EightTrigrams.tMaxLoad || p7On == true || p5On == false || Main.hardMode == false)
                return;

            p7On = true;
            SoundEngine.PlaySound(Selected);
            EightTrigrams.tLoad += 4;
            txt1.Remove();
            EightTrigrams.infoImg.Remove();
        }

        private void SetRectangle(UIElement uiElmt, float left, float top, float width, float height)
        {
            uiElmt.Left.Set(left, 0f);
            uiElmt.Top.Set(top, 0f);
            uiElmt.Width.Set(width, 0f);
            uiElmt.Height.Set(height, 0f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (EightTrigrams.tType != 1 || EightTrigrams.vsb == false)
                return;

            base.Draw(spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            if (EightTrigrams.tType != 1 || EightTrigrams.vsb == false)
                return;

            if (p1On != true)
            {
                t1p0 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p0 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P0 = new ETButton(t1p0, "T1P0");

            SetRectangle(playT1P0, left: 504f, top: 110f, width: 14f, height: 14f);

            playT1P0.OnMouseOver += new MouseEvent(T1P0Info);
            playT1P0.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P0.OnLeftClick += new MouseEvent(T1P0On);

            t1Elmt.Append(playT1P0);


            if (p2On != true)
            {
                t1p1 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p1 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P1 = new ETButton(t1p1, "T1P1");

            SetRectangle(playT1P1, left: 458f, top: 126f, width: 14f, height: 14f);

            playT1P1.OnMouseOver += new MouseEvent(T1P1Info);
            playT1P1.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P1.OnLeftClick += new MouseEvent(T1P1On);

            t1Elmt.Append(playT1P1);



            if (p3On != true)
            {
                t1p2 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p2 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P2 = new ETButton(t1p2, "T1P2");

            SetRectangle(playT1P2, left: 550f, top: 122f, width: 14f, height: 14f);

            playT1P2.OnMouseOver += new MouseEvent(T1P2Info);
            playT1P2.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P2.OnLeftClick += new MouseEvent(T1P2On);

            t1Elmt.Append(playT1P2);


            if (p4On != true)
            {
                t1p3 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p3 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P3 = new ETButton(t1p3, "T1P3");

            SetRectangle(playT1P3, left: 432f, top: 162f, width: 14f, height: 14f);

            playT1P3.OnMouseOver += new MouseEvent(T1P3Info);
            playT1P3.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P3.OnLeftClick += new MouseEvent(T1P3On);

            t1Elmt.Append(playT1P3);


            if (p5On != true)
            {
                t1p4 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p4 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P4 = new ETButton(t1p4, "T1P4");

            SetRectangle(playT1P4, left: 584f, top: 160f, width: 14f, height: 14f);

            playT1P4.OnMouseOver += new MouseEvent(T1P4Info);
            playT1P4.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P4.OnLeftClick += new MouseEvent(T1P4On);

            t1Elmt.Append(playT1P4);


            if (p6On != true)
            {
                t1p5 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p5 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P5 = new ETButton(t1p5, "T1P5");

            SetRectangle(playT1P5, left: 432f, top: 218f, width: 14f, height: 14f);

            playT1P5.OnMouseOver += new MouseEvent(T1P5Info);
            playT1P5.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P5.OnLeftClick += new MouseEvent(T1P5On);

            t1Elmt.Append(playT1P5);


            if (p7On != true)
            {
                t1p6 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/Point");
            }
            else
            {
                t1p6 = ModContent.Request<Texture2D>("Conquest/Assets/GUI/PointOn");
            }

            ETButton playT1P6 = new ETButton(t1p6, "T1P6");

            SetRectangle(playT1P6, left: 584f, top: 218f, width: 14f, height: 14f);

            playT1P6.OnMouseOver += new MouseEvent(T1P6Info);
            playT1P6.OnMouseOut += new MouseEvent(T1P0InfoClose);

            playT1P6.OnLeftClick += new MouseEvent(T1P6On);

            t1Elmt.Append(playT1P6);


            base.Update(gameTime);
        }
    }

    internal class ETPoint : SoundlessButton
    {
        internal string etPHv;
        public ETPoint(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            this.etPHv = hoverText;
        }
    }
    class T1UISystem : ModSystem
    {
        private UserInterface t1Itfc;

        internal T1 eT1;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                eT1 = new();
                t1Itfc = new();
                t1Itfc.SetState(eT1);
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            t1Itfc?.Update(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int etIdx = layers.FindIndex(layer => layer.Name.Equals("Conquest: EightTrigrams")) + 1;
            if (etIdx != -1)
            {
                layers.Insert(etIdx, new LegacyGameInterfaceLayer(
                    "Conquest: Trigram1",
                    delegate
                    {
                        t1Itfc.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                    )
                );
            }
        }
    }
}
