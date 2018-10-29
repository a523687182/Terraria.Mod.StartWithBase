﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.GameContent.UI.States;
using Terraria.World.Generation;
using Terraria.GameContent.UI.Elements;
using Terraria.UI.Gamepad;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.ModLoader;
using System.Reflection;

namespace StartWithBase
{
    class StartWithBaseUI : UIPanel
    {


        Dictionary<string, UIScalableImageButtton> buttonDict;


        UIPanel settingsPannel;
        public UIText counterText;
        UIScalableImageButtton startIcon;
        public bool doNotBuildBase;
        UIGenProgressBar pbar;

        public StartWithBaseUI(UIState uistate, Mod mod)
        {
            FieldInfo field = typeof(UIWorldLoad).GetField("_progressBar", BindingFlags.Instance | BindingFlags.NonPublic);
            if(field != null)
                pbar = (UIGenProgressBar)field.GetValue(uistate);
            
            doNotBuildBase = false;


            settingsPannel = new UIPanel();
            settingsPannel.Left.Precent = 2.025f;
            settingsPannel.Top.Precent = 0.025f;
            settingsPannel.Width.Set(0f, 0.95f);
            settingsPannel.Height.Set(0f, 0.95f);
            settingsPannel.SetPadding(0);
            settingsPannel.BackgroundColor = new Color(73, 94, 171);

            buttonDict = new Dictionary<string, UIScalableImageButtton>();

            string content = "base";
            UIScalableImageButtton base2Button = new UIScalableImageButtton(mod.GetTexture("images/base2"), content);
            base2Button.Id = "ba2";
            buttonDict.Add(base2Button.Id, base2Button);

            UIScalableImageButtton base3Button = new UIScalableImageButtton(mod.GetTexture("images/base3"), content);
            base3Button.Id = "ba3";
            buttonDict.Add(base3Button.Id, base3Button);

            UIScalableImageButtton base3bButton = new UIScalableImageButtton(mod.GetTexture("images/base3b"), content);
            base3bButton.Id = "b3b";
            buttonDict.Add(base3bButton.Id, base3bButton);

            UIScalableImageButtton base4Button = new UIScalableImageButtton(mod.GetTexture("images/base4e"), content);
            base4Button.Id = "ba4";
            buttonDict.Add(base4Button.Id, base4Button);

            UIScalableImageButtton base6Button = new UIScalableImageButtton(mod.GetTexture("images/base6"), content);
            base6Button.Id = "ba6";
            buttonDict.Add(base6Button.Id, base6Button);


            content = "tile";
            foreach (var sty in BuildBase.TileTypeDict)
            {
                UIScalableImageButtton fur = new UIScalableImageButtton(ModLoader.GetTexture("Terraria/Item_" + sty.Value.ItemID), content);
                fur.Id = sty.Key;
                buttonDict.Add(fur.Id, fur);
            }
            content = "wall";
            foreach (var sty in BuildBase.wallTypeDict)
            {
                UIScalableImageButtton fur = new UIScalableImageButtton(ModLoader.GetTexture("Terraria/Item_" + sty.Value.ItemIDofWallType), content);
                fur.Id = sty.Key;
                buttonDict.Add(fur.Id, fur);
            }

            content = "furniture";
            foreach (var sty in BuildBase.styleTypeDict)
            {
                UIScalableImageButtton fur = new UIScalableImageButtton(ModLoader.GetTexture("Terraria/Item_" + sty.Value.ItemID), content);
                fur.Id = sty.Key;
                buttonDict.Add(fur.Id, fur);
            }

            content = "lantern";
            foreach (var sty in BuildBase.lanternTypeDict)
            {
                UIScalableImageButtton fur = new UIScalableImageButtton(ModLoader.GetTexture("Terraria/Item_" + sty.Value.ItemID), content);
                fur.Id = sty.Key;
                buttonDict.Add(fur.Id, fur);
            }
            content = "platform";
            foreach (var sty in BuildBase.platformTypeDict)
            {
                UIScalableImageButtton fur = new UIScalableImageButtton(ModLoader.GetTexture("Terraria/Item_" + sty.Value.ItemID), content);
                fur.Id = sty.Key;
                buttonDict.Add(fur.Id, fur);
            }

            content = "numbers";
            UIScalableImageButtton rand = new UIScalableImageButtton(mod.GetTexture("images/qm"), content);
            rand.Id = "random";
            buttonDict.Add(rand.Id, rand);
            for (int num = 0; num < 10; num++)
            {
                UIScalableImageButtton fur = new UIScalableImageButtton(mod.GetTexture("images/" + num), content);
                fur.Id = "config" + num;
                buttonDict.Add(fur.Id, fur);
            }
            UIScalableImageButtton save = new UIScalableImageButtton(mod.GetTexture("images/configSave"), content);
            save.Id = "configSave";
            buttonDict.Add(save.Id, save);

            counterText = new UIText("42", 2, true);
            settingsPannel.Append(counterText);
            counterText.HAlign = 0.0f;
            counterText.VAlign = 1.0f;
            counterText.PaddingBottom = paddingY;
            counterText.MarginLeft = padding;

            foreach (var but in buttonDict)
            {
                UIScalableImageButtton butt = buttonDict[but.Key];
                settingsPannel.Append(butt);
                butt.OnClick += clickBase;
                butt.OnRightClick += rightClickPannel;
                //butt.SetVisibility(0.8f, 0.8f);
            }


            InitButtons();
            setSize();
            uistate.Append(settingsPannel);
            settingsPannel.OnRightClick += rightClickPannel;
            uistate.Append(settingsPannel);
                        
            startIcon = new UIScalableImageButtton(Texture2D.FromStream(Main.instance.GraphicsDevice, new MemoryStream(mod.GetFileBytes("icon.png"))), content);
            startIcon.Height.Pixels *= 2;
            startIcon.Width.Pixels *= 2;
            startIcon.PaddingRight = -startIcon.Width.Pixels;
            startIcon.PaddingTop = -startIcon.Height.Pixels;
            startIcon.VAlign = 0f;
            startIcon.HAlign = 1f;
            startIcon.OnClick += leftClickicon;
            startIcon.OnRightClick += rightClickicon;
            uistate.Append(startIcon);
            //uistate.Append(this);

            lastSave = -1;
        }

        private void InitButtons()
        {
            if (Int32.Parse(counterText.Text) == 0)
                return;

            foreach (var but in buttonDict)
            {
                string content = but.Value.content;

                UIScalableImageButtton butt = buttonDict[but.Key];
                if (content.Equals("base"))
                {
                    //todo something better
                    if (butt.Id.Equals("ba2") && BuildBase.baseType == BuildBase.BaseType.Base2)
                        but.Value.isClicked = true;
                    else if (butt.Id.Equals("ba3") && BuildBase.baseType == BuildBase.BaseType.Base3)
                        but.Value.isClicked = true;
                    else if (butt.Id.Equals("b3b") && BuildBase.baseType == BuildBase.BaseType.Base3ext)
                        but.Value.isClicked = true;
                    else if (butt.Id.Equals("ba4") && BuildBase.baseType == BuildBase.BaseType.Base4)
                        but.Value.isClicked = true;
                    else if (butt.Id.Equals("ba6") && BuildBase.baseType == BuildBase.BaseType.Base6)
                        but.Value.isClicked = true;
                }
                else if (content.Equals("tile"))
                {
                    int type = BuildBase.TileTypeDict.ContainsKey(but.Value.Id) ? BuildBase.TileTypeDict[but.Value.Id].TileID : -1;
                    if (BuildBase.curTileType == type)
                        but.Value.isClicked = true;
                }
                else if (content.Equals("wall"))
                {
                    byte type = (byte)(BuildBase.wallTypeDict.ContainsKey(but.Value.Id) ? BuildBase.wallTypeDict[but.Value.Id].WallID : 0);
                    if (BuildBase.curWallType == type)
                        but.Value.isClicked = true;
                }
                else if (content.Equals("furniture"))
                {
                    //may also selected it custom style contains this workbench
                    int type = BuildBase.styleTypeDict.ContainsKey(but.Value.Id) ? BuildBase.styleTypeDict[but.Value.Id].WorkBenchStyle : -1;
                    if (BuildBase.curWorkBenchStyle == type)
                        but.Value.isClicked = true;
                }
                else if (content.Equals("lantern"))
                {
                    int type = BuildBase.lanternTypeDict.ContainsKey(but.Value.Id) ? BuildBase.lanternTypeDict[but.Value.Id].LanternStyle : -1;
                    if (BuildBase.curLanternStyle == type)
                        but.Value.isClicked = true;
                }
                else if (content.Equals("platform"))
                {
                    int type = BuildBase.platformTypeDict.ContainsKey(but.Value.Id) ? BuildBase.platformTypeDict[but.Value.Id].PlatformStyle : -1;
                    if (BuildBase.curPlatformStyle == type)
                        but.Value.isClicked = true;
                }
            }

        }

        public void free()
        {
            if (pannelActive)
                swapView();

           
            settingsPannel.RemoveAllChildren();
            settingsPannel.Remove();
            settingsPannel = null;
           
            startIcon.RemoveAllChildren();
            startIcon.Remove();
            startIcon = null;

            buttonDict.Clear();
            buttonDict = null;

            pbar = null;

            
        }

        public void setRandom()
        {
            Random rnd = new Random();
            string rand = "world$";

            rand += BuildBase.baseTypeDict.ElementAt(rnd.Next(0, BuildBase.baseTypeDict.Count)).Key ;
            rand += BuildBase.TileTypeDict.ElementAt(rnd.Next(0, BuildBase.TileTypeDict.Count)).Key;
            rand += BuildBase.wallTypeDict.ElementAt(rnd.Next(0, BuildBase.wallTypeDict.Count)).Key;
            rand += BuildBase.styleTypeDict.ElementAt(rnd.Next(0, BuildBase.styleTypeDict.Count)).Key;
            rand += BuildBase.lanternTypeDict.ElementAt(rnd.Next(0, BuildBase.lanternTypeDict.Count)).Key;
            rand += BuildBase.platformTypeDict.ElementAt(rnd.Next(0, BuildBase.platformTypeDict.Count)).Key;

            BuildBase.parsWN(rand, false);
            InitButtons();

        }


        public string getValues()
        {
            string wn = "world$";
            foreach (var but in buttonDict)
            {
                if (but.Value.isClicked)
                {
                    string pa = but.Value.Id;
                    if (BuildBase.styleTypeDict.ContainsKey(pa) || BuildBase.wallTypeDict.ContainsKey(pa) || BuildBase.TileTypeDict.ContainsKey(pa) ||
                        BuildBase.lanternTypeDict.ContainsKey(pa) || BuildBase.baseTypeDict.ContainsKey(pa) || BuildBase.platformTypeDict.ContainsKey(pa))
                        wn += pa;
                }


            }

            return wn;
        }

        int lastUp = 0;
        const float padding = 16f;
        const float paddingY = 32f;
        public void setSize()
        {
            if (settingsPannel == null) return;

            const int numB = 5; // bases
            float spacing = 0;
            float spacingY = 0;


            float maxW = (Main.screenWidth * settingsPannel.Width.Precent - (numB + 1) * padding) / numB;
            float maxWN = (Main.screenWidth * settingsPannel.Width.Precent * 0.5f - (12 + 1) * padding) / 10f;

            string content = buttonDict.ElementAt(0).Value.content;
            float maxHei = 0;
            foreach (var but in buttonDict)
            {
                string contentThis = but.Value.content;
                if (!content.Equals(contentThis))
                {
                    if (!content.Equals("base"))
                    {
                        center(content, spacing);
                    }
                    spacing = 0;
                    spacingY += maxHei + paddingY;

                    if (contentThis.Equals("numbers"))
                        spacingY += paddingY;

                    content = contentThis;
                    maxHei = 0;
                }

                UIScalableImageButtton butt = buttonDict[but.Key];

                if (content.Equals("base"))
                {
                    float fac = butt.Width.Pixels / maxW;
                    butt.Width.Pixels = maxW;
                    butt.Height.Pixels /= fac;
                }
                else if (content.Equals("numbers"))
                {
                    float fac = butt.Width.Pixels / (maxWN);
                    butt.Width.Pixels = maxWN;
                    butt.Height.Pixels /= fac;
                }
                else
                {
                    butt.Width.Pixels *= 2; butt.Height.Pixels *= 2;
                }

                butt.MarginLeft = spacing + padding;
                butt.MarginTop = spacingY + padding;
                spacing += butt.Width.Pixels + padding;
                maxHei = Math.Max(maxHei, butt.Height.Pixels);
            }
            center(content, spacing);
            centerBot(content, spacingY, paddingY);
            lastUp = Main.screenWidth;
        }



        private void center(string content, float spacing)
        {
            //todo something better
            float place = 0.5f * ((Main.screenWidth * settingsPannel.Width.Precent) - spacing);
            foreach (var row in buttonDict)
            {
                if (row.Value.content.Equals(content))
                {
                    row.Value.MarginLeft += place;
                }
            }
        }
        private void centerBot(string content, float spacingY, float padBot)
        {
            //todo something better
            float place = ((Main.screenHeight * settingsPannel.Height.Precent) - spacingY) - padBot;
            foreach (var row in buttonDict)
            {
                if (row.Value.content.Equals(content))
                {
                    row.Value.MarginTop += place - row.Value.Height.Pixels;
                }
            }
        }


        //toDo one for all
        private void setOtherOff(string clickedB, string content, bool offAll = false)
        {
            foreach (var but in buttonDict)
            {
                if ((!but.Key.Equals(clickedB) && content.Equals(but.Value.content)) || offAll)
                {
                    buttonDict[but.Key].isClicked = false;
                    buttonDict[but.Key].SetVisibility(0.8f, 0.8f);
                }
            }

        }

        private string findActive(string content)
        {
            foreach (var but in buttonDict)
            {
                if (but.Value.isClicked && content.Equals(but.Value.content))
                {
                    return but.Value.Id;
                }
            }
            return "";
        }

        private int lastSave;
        private void clickBase(UIMouseEvent evt, UIElement listeningElement)
        {
            string bid = (listeningElement as UIScalableImageButtton).Id;

            if (bid.Contains("config") )
            {
                if (bid.Length == ("config").Length + 1)
                {
                    int which = Int32.Parse(bid.Substring(6));
                    bool doExist = BuildBase.ReadAndSetConfig(bid.Substring(6));
                    if (doExist)
                    {
                        setOtherOff("", "", true);
                        InitButtons();
                    }
                    else
                    {
                        Main.PlaySound(SoundID.Frog);
                    }
                }
                else
                {
                    string active = findActive("numbers");
                                        
                    int num = lastSave;
                    if (active.Length > 0)
                    {
                        if (active.Length == ("config").Length + 1)
                        {
                            active = active.Substring(active.Length - 1);
                            bool pars = Int32.TryParse(active, out num);

                            if (!pars) num = lastSave;
                        }
                    }
                    if (num != -1)
                    {
                        BuildBase.parsWN(getValues());
                        BuildBase.CheckGenerate(num.ToString(), true);
                        lastSave = num;
                    }
                    

                }
            }
            if (bid.Equals("random"))
            {
                setOtherOff("", "", true);
                setRandom();
            }
            else
            {
                buttonDict[bid].isClicked = true;
            }
            buttonDict[bid].SetVisibility(1.0f, 1.0f);
            setOtherOff(bid, buttonDict[bid].content);
        }

        bool pannelActive = false;
        private void swapView()
        {
            pannelActive = !pannelActive;
            
            settingsPannel.Left.Precent += (pannelActive ? -2f : 2f);
            settingsPannel.Recalculate();

            startIcon.Left.Precent += (!pannelActive ? -2f : 2f);
            startIcon.Recalculate();

            if (pbar != null)
            {
                pbar.Left.Precent += (pannelActive ? 2f : -2f); ;
                pbar.Recalculate();
            }

        }


        private void rightClickPannel(UIMouseEvent evt, UIElement listeningElement)
        {
            swapView();
        }

        private void leftClickicon(UIMouseEvent evt, UIElement listeningElement)
        {
            swapView();
        }

        private void rightClickicon(UIMouseEvent evt, UIElement listeningElement)
        {
            startIcon.Left.Precent += 2f;
            startIcon.Recalculate();
            doNotBuildBase = true;
        }

        public void Init()
        {

        }

        /*
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

        }

        public override void Update(GameTime gameTime)
        {
           
        }*/


    }
}