using PriorityMod.Core;
using PriorityMod.CrossCompat;
using PriorityMod.Extensions;
using PriorityMod.Settings;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace PriorityMod.Tools
{

	public class Listing_PriorityMod : Listing_Standard
	{

		public Listing_PriorityMod(GameFont font) : base(font) { }

		public Listing_PriorityMod() : base() { }

		public string TextFieldColored(string text, Color color)
		{
			Rect rect = base.GetRect(Text.LineHeight);
			Color save = GUI.backgroundColor;
			GUI.backgroundColor = color;
			string result = Widgets.TextField(rect, text);
			base.Gap(this.verticalSpacing);
			GUI.backgroundColor = save;
			return result;
		}

		public int ColorList(int current, int total, Func<int, Color> function, int size = 24, int padding = 2, int borderThickness = 2)
		{
			Rect rect = GetRect(size);
			GUI.BeginGroup(rect);
			if (total == 0)
			{
				GUI.EndGroup();
				return 0;
			}

			int unselectedSize = size - (borderThickness * 2);
			int amount = (int)Math.Floor((rect.width + padding) / (size + padding));

			int first = current - (amount / 2);
			if (first < 0)
			{
				first = 0;
			}
			float start = 0;
			int last = first + amount;
			if (last > total)
			{
				int diff = last - total;
				last -= diff;
				if (diff < first)
				{
					first -= diff;
				}
				else
				{
					diff -= first;
					amount -= diff;
					first = 0;
					start = (rect.width - ((size + padding) * last) - padding) / 2;
				}
			}
			int result = current;
			for (int i = 0; i < amount; i++)
			{
				float x = 6 + start + (i * size) + (i * padding);
				int id = first + i;
				Color color = function.Invoke(id);
				Rect boxRect = new Rect(x - borderThickness, 0, size, size);
				if (id == current)
				{
					Port.Widgets_DrawBoxSolidWithOutline(boxRect, color, Color.white, borderThickness);
					continue;
				}
				Widgets.DraggableResult dragResult = Widgets.ButtonInvisibleDraggable(boxRect, true);
				if (Mouse.IsOver(boxRect))
				{
					Port.Widgets_DrawBoxSolidWithOutline(boxRect, color, Color.gray, borderThickness);
					if (dragResult == Widgets.DraggableResult.Pressed || dragResult == Widgets.DraggableResult.Dragged)
					{
						result = id;
					}
					continue;
				}
				Widgets.DrawBoxSolid(new Rect(x, borderThickness, unselectedSize, unselectedSize), color);
			}
			GUI.EndGroup();
			return result;
		}

	}

	public class ColorPicker
	{ 

		private static readonly Regex HEX_REGEX = new Regex("^#[a-fA-F0-9]{1,6}$");

		public float Height
        {
			get => totalHeight;
        }

		private Texture2D pickerTex;

		private Texture2D saturationTex;
		private Texture2D lightnessTex;
		private Texture2D hueTex;

		private Texture2D redTex;
		private Texture2D greenTex;
		private Texture2D blueTex;

		private readonly float pickerHeight;
		private readonly float barHeight;
		private readonly float gapSpace;
		private readonly float barSpace;

		private readonly float selectionSize;
		private readonly float selectionHalfSize;

		private readonly float totalHeight;
		private readonly float labelSize;

		private int prevColorId;
		private float prevPickerSaturation;

		private float prevHue;
		private float prevSaturation;
		private float prevLightness;

		private float prevRed;
		private float prevGreen;
		private float prevBlue;

		private string hexBuf = null;

		public ColorPicker(float pickerHeight, float barHeight, float selectionSize, float gapSpace, float barSpace, float labelSize)
		{
			this.pickerHeight = pickerHeight;
			this.selectionSize = selectionSize;
			this.selectionHalfSize = selectionSize / 2f;
			this.barHeight = barHeight;
			this.gapSpace = gapSpace;
			this.barSpace = barSpace;
			this.labelSize = labelSize;
			this.totalHeight = pickerHeight + 3 * gapSpace + 7 * barHeight + 4 * barSpace;
		}

		public void Draw(Listing listing, in DisplayBuffer buffer)
		{
			bool update = false;
			if (prevColorId != buffer.selected)
            {
				prevColorId = buffer.selected;
				update = true;
				hexBuf = null;
				Log.Message("Changed: " + buffer.Hex);
			}

			float pHeight = pickerHeight;
			float pWidth = pHeight * 3.6f;
			if (pWidth > listing.ColumnWidth)
			{
				pWidth = listing.ColumnWidth * 0.8f;
				pHeight = pWidth / 3.6f;
			}

			Rect root = listing.GetRect(totalHeight);

			float hue = buffer.Hue;
			float saturation = buffer.Saturation;
			float lightness = buffer.Lightness;
			float red = buffer.RedF;
			float green = buffer.GreenF;
			float blue = buffer.BlueF;

			bool updateTextures = prevHue != hue || prevSaturation != saturation || prevLightness != lightness || prevRed != red || prevGreen != green || prevBlue != blue;
            if (updateTextures)
            {
				prevHue = hue;
				prevSaturation = saturation;
				prevLightness = lightness;
				prevRed = red;
				prevGreen = green;
				prevBlue = blue;
            }

			float rHue = hue;
			float rSaturation = saturation;
			float rLightness = lightness;
			float rRed = red;
			float rGreen = green;
			float rBlue = blue;

			GUI.BeginGroup(root);

			float pAlignX = ((root.width - pWidth - pHeight - gapSpace) / 2) + pHeight + gapSpace;
			float pSpaceX = pAlignX - pHeight - gapSpace;

			Widgets.DrawBoxSolid(new Rect(pSpaceX, 0, pHeight, pHeight), buffer.Color);

			if (DrawPicker(pAlignX, pWidth, pHeight, ref hue, saturation, ref lightness) && !update)
            {
				update = true;
				buffer.Hue = hue;
				buffer.Lightness = lightness;
            }

			float y = gapSpace + pHeight;
			if (DrawBar(pAlignX, y, pWidth, prevHue, ref hue, updateTextures, ref hueTex, (value) => Utils.FromHSL(value, rSaturation, rLightness)) && !update)
            {
				update = true;
				buffer.Hue = hue;
			}
			int numBuf = (int) Math.Round(prevHue * 360f);
			string txtBuf = numBuf.ToString();
			if (WidgetUtil.NumericTextField(new Rect(pSpaceX, y, pHeight, barHeight), labelSize, "color.hue".Translate(), ref numBuf, 0, 360) && !update)
			{
				update = true;
				buffer.Hue = (numBuf / 360f);
			}
			y += barSpace + barHeight;

			if (DrawBar(pAlignX, y, pWidth, prevSaturation, ref saturation, updateTextures, ref saturationTex, (value) => Utils.FromHSL(rHue, value, rLightness)) && !update)
            {
				update = true;
				buffer.Saturation = saturation;
            }
			numBuf = (int)Math.Round(prevSaturation * 100f);
			txtBuf = numBuf.ToString();
			if (WidgetUtil.NumericTextField(new Rect(pSpaceX, y, pHeight, barHeight), labelSize, "color.saturation".Translate(), ref numBuf, 0, 100) && !update)
            {
				update = true;
				buffer.Saturation = (numBuf / 100f);
			}
			y += barSpace + barHeight;

			if (DrawBar(pAlignX, y, pWidth, prevLightness, ref lightness, updateTextures, ref lightnessTex, (value) => Utils.FromHSL(rHue, rSaturation, value)) && !update)
			{
				update = true;
				buffer.Lightness = lightness;
			}
			numBuf = (int)Math.Round(prevLightness * 100f);
			txtBuf = numBuf.ToString();
			if (WidgetUtil.NumericTextField(new Rect(pSpaceX, y, pHeight, barHeight), labelSize, "color.lightness".Translate(), ref numBuf, 0, 100) && !update)
			{
				update = true;
				buffer.Lightness = (numBuf / 100f);
			}
			y += gapSpace + barHeight;


			if (DrawBar(pAlignX, y, pWidth, prevRed, ref red, updateTextures, ref redTex, (value) => new Color(value, rGreen, rBlue)) && !update)
			{
				update = true;
				buffer.RedF = red;
			}
			numBuf = prevRed.RGBToInt();
			txtBuf = numBuf.ToString();
			if (WidgetUtil.NumericTextField(new Rect(pSpaceX, y, pHeight, barHeight), labelSize, "color.red".Translate(), ref numBuf, 0, 255) && !update)
            {
				update = true;
				buffer.Red = numBuf;
			}
			y += barSpace + barHeight;

			if (DrawBar(pAlignX, y, pWidth, prevGreen, ref green, updateTextures, ref greenTex, (value) => new Color(rRed, value, rBlue)) && !update)
			{
				update = true;
				buffer.GreenF = green;
			}
			numBuf = prevGreen.RGBToInt();
			txtBuf = numBuf.ToString();
			if (WidgetUtil.NumericTextField(new Rect(pSpaceX, y, pHeight, barHeight), labelSize, "color.green".Translate(), ref numBuf, 0, 255) && !update)
            {
				update = true;
				buffer.Green = numBuf;
			}
			y += barSpace + barHeight;

			if (DrawBar(pAlignX, y, pWidth, prevBlue, ref blue, updateTextures, ref blueTex, (value) => new Color(rRed, rGreen, value)) && !update)
			{
				update = true;
				buffer.BlueF = blue;
			}
			numBuf = prevBlue.RGBToInt();
			txtBuf = numBuf.ToString();
			if (WidgetUtil.NumericTextField(new Rect(pSpaceX, y, pHeight, barHeight), labelSize, "color.blue".Translate(), ref numBuf, 0, 255) && !update)
			{
				update = true;
				buffer.Blue = numBuf;
			}
			y += gapSpace + barHeight;

			txtBuf = buffer.Hex; 
			if (hexBuf == null || (update && !hexBuf.Equals(txtBuf)))
			{
				hexBuf = txtBuf;
			}
			if (WidgetUtil.TextField(new Rect(pSpaceX, y, pHeight + gapSpace + pWidth, barHeight), labelSize, "color.hex".Translate(), ref hexBuf, ref txtBuf, ValidateHex, maxLength: 7) && !update)
			{
				buffer.Hex = txtBuf;
			}

			GUI.EndGroup();

		}

		private bool ValidateHex(string hex)
        {
			int length = hex.Length;
			if(length != 2 && length != 4 && length != 7)
            {
				return false;
            }
			return HEX_REGEX.IsMatch(hex);
        }

		private bool DrawPicker(float x, float width, float height, ref float hue, float saturation, ref float lightness)
		{
			if(TextureUtil.UpdateTexture(ref pickerTex, ref prevPickerSaturation, saturation, width, height))
			{
				UpdatePickerTexture(ref pickerTex, prevPickerSaturation);
			}
			Rect box = new Rect(x, 0, width, height);
			GUI.DrawTexture(box, pickerTex, ScaleMode.ScaleToFit);

			Color prev = GUI.color;
			GUI.color = lightness > 0.25f ? Color.black : Color.white;
			GUI.DrawTexture(new Rect(box.x + hue * width - selectionHalfSize, (1f - lightness) * height - selectionHalfSize, selectionSize, selectionSize), UIResources.SelectionCircle, ScaleMode.ScaleToFit, true, 1f);
			GUI.color = prev;
			if(WidgetUtil.IsDraggedXY(box, out float progressX, out float progressY))
            {
				hue = progressX;
				lightness = 1f - progressY;
				return true;
            }
			return false;
		}

		private bool DrawBar(float x, float y, float width, float previousValue, ref float currentValue, bool updateTexture, ref Texture2D texture, Func<float, Color> func)
        {
			if(TextureUtil.UpdateTexture(ref texture, updateTexture, width, barHeight))
            {
				UpdateBarTexture(ref texture, func);
			}
			Rect box = new Rect(x, y, width, barHeight);
			GUI.DrawTexture(box, texture, ScaleMode.ScaleToFit);
			Color prev = GUI.color;
			GUI.color = Color.white;
			Widgets.DrawBox(new Rect(x + (previousValue * width) - selectionHalfSize, y, selectionSize, barHeight), 1);
			GUI.color = prev;
			if (WidgetUtil.IsDraggedX(box, out float value))
			{
				currentValue = value;
				return true;
			}
			return false;
		}


		private void UpdatePickerTexture(ref Texture2D texture, float saturation)
		{
			float hueMax = texture.width;
			float lightMax = texture.height;
			for (int x = 0; x < texture.width; x++)
			{
				for (int y = 0; y < texture.height; y++)
				{
					texture.SetPixel(x, y, Utils.FromHSL(x / hueMax, saturation, y / lightMax));
				}
			}
			texture.Apply();
		}

		private void UpdateBarTexture(ref Texture2D texture, Func<float, Color> func)
		{
			float valMax = texture.width;
			for (int x = 0; x < texture.width; x++)
			{
				Color color = func.Invoke(x / valMax);
				for (int y = 0; y < texture.height; y++)
				{
					texture.SetPixel(x, y, color);
				}
			}
			texture.Apply();
		}

	}

	public static class WidgetUtil
    {

		private static int dragId;

		public static void ResetDragId()
        {
			dragId = 0;
        }

		public static bool TextField(Rect rect, float labelWidth, string label, ref string buffer, ref string value, Func<string, bool> validator, float gapSpace = 4f, int maxLength = 0)
		{
			Widgets.Label(new Rect(rect.x, rect.y, labelWidth, rect.height + 4), label);
			buffer = Widgets.TextField(new Rect(rect.x + labelWidth + gapSpace, rect.y, rect.width - labelWidth - gapSpace, rect.height), buffer);
			if (maxLength != 0 && buffer.Length > maxLength)
			{
				buffer = buffer.Substring(0, maxLength);
			}
            if (validator.Invoke(buffer))
            {
				value = buffer;
				return true;
            }
			return false;
		}

		public static bool NumericTextField(Rect rect, float labelWidth, string label, ref int reference, int min = 0, int max = int.MaxValue, float gapSpace = 4f)
		{
			Widgets.Label(new Rect(rect.x, rect.y, labelWidth, rect.height + 4), label);
			string text = reference.ToString();
			int value = reference;
			Widgets.TextFieldNumeric(new Rect(rect.x + labelWidth + gapSpace, rect.y, rect.width - labelWidth - gapSpace, rect.height), ref value, ref text, min, max);
			if (text == "")
			{
				value = 0;
			}
			if(reference != value)
			{
				reference = value;
				return true;
            }
			return false;
		}

		public static bool IsDraggedX(Rect rect, out float progressX)
		{
			int id = UI.GUIToScreenPoint(new Vector2(rect.x, rect.y)).GetHashCode();
			id = Gen.HashCombine<float>(id, rect.width);
			id = Gen.HashCombine<float>(id, rect.height);
			if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect) && WidgetUtil.dragId != id)
			{
				WidgetUtil.dragId = id;
				Event.current.Use();
			}
			if (WidgetUtil.dragId == id && Port.UnityGUIBugsFixer_MouseDrag(0))
			{
				progressX = Mathf.Clamp((Event.current.mousePosition.x - rect.x) / rect.width, 0f, 1f);
				if (Event.current.type == EventType.MouseDrag)
				{
					Event.current.Use();
				}
				return true;
			}
			progressX = 0;
			return false;
		}

		public static bool IsDraggedXY(Rect rect, out float progressX, out float progressY)
		{
			int id = UI.GUIToScreenPoint(new Vector2(rect.x, rect.y)).GetHashCode();
			id = Gen.HashCombine<float>(id, rect.width);
			id = Gen.HashCombine<float>(id, rect.height);
			if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect) && WidgetUtil.dragId != id)
			{
				WidgetUtil.dragId = id;
				Event.current.Use();
			}
			if (WidgetUtil.dragId == id && Port.UnityGUIBugsFixer_MouseDrag(0))
			{
				progressX = Mathf.Clamp((Event.current.mousePosition.x - rect.x) / rect.width, 0f, 1f);
				progressY = Mathf.Clamp((Event.current.mousePosition.y - rect.y) / rect.height, 0f, 1f);
				if (Event.current.type == EventType.MouseDrag)
				{
					Event.current.Use();
				}
				return true;
			}
			progressX = progressY = 0;
			return false;
		}


    }

	public static class TextureUtil
	{

		public static bool UpdateTexture(ref Texture2D texture, ref float previousValue, float currentValue, float width, float height)
		{
			int texWidth = (int)Math.Round(width);
			int texHeight = (int)Math.Round(height);
			if (texture == null)
			{
				texture = new Texture2D(texWidth, texHeight);
				return true;
			} else if(texture.width != texWidth || texture.height != texHeight)
            {
				texture.Resize(texWidth, texHeight);
				return true;
            } else if(previousValue != currentValue)
            {
				previousValue = currentValue;
				return true;
            }
			return false;
		}

		public static bool UpdateTexture(ref Texture2D texture, bool updateTexture, float width, float height)
		{
			int texWidth = (int)Math.Round(width);
			int texHeight = (int)Math.Round(height);
			if (texture == null)
			{
				texture = new Texture2D(texWidth, texHeight);
				return true;
			}
			else if (texture.width != texWidth || texture.height != texHeight)
			{
				texture.Resize(texWidth, texHeight);
				return true;
			}
			return updateTexture;
		}

	}

}
