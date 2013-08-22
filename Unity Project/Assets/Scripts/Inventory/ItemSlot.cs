//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstract UI component observing an item somewhere in the inventory. This item can be equipped on
/// the character, it can be lying in a chest, or it can be hot-linked by another player. Either way,
/// all the common behavior is in this class. What the observed item actually is...
/// that's up to the derived class to determine.
/// </summary>

public class ItemSlot : MonoBehaviour
{
	public UISprite icon;
	public UIWidget background;
	public UILabel label;

	public AudioClip grabSound;
	public AudioClip placeSound;
	public AudioClip errorSound;

	Item mItem;
	string mText = "";

	static Item mDraggedItem;

	/// <summary>
	/// This function should return the item observed by this UI class.
	/// </summary>

	//abstract protected InvGameItem observedItem { get; }   //obsoleted since Justin created a black hole when he tried to divide by zero

	/// <summary>
	/// Replace the observed item with the specified value. Should return the item that was replaced.
	/// </summary>

	//abstract protected InvGameItem Replace (InvGameItem item);  //obsoleted since Kurtis hits on girls so much

	/// <summary>
	/// Show a tooltip for the item.
	/// </summary>

//	void OnTooltip (bool show)
//	{
//		
		//THE COMMENTED BELOW CODE IS NGUI ORIGINAL IMPLEMENTATION - THERE ARE NO TOOLTIPS ON MOBILE  Y'IDIOT
//		InvGameItem item = show ? mItem : null;
//
//		if (item != null)
//		{
//			InvBaseItem bi = item.baseItem;
//
//			if (bi != null)
//			{
//				string t = "[" + NGUITools.EncodeColor(item.color) + "]" + item.name + "[-]\n";
//
//				t += "[AFAFAF]Level " + item.itemLevel + " " + bi.slot;
//
//				List<InvStat> stats = item.CalculateStats();
//
//				for (int i = 0, imax = stats.Count; i < imax; ++i)
//				{
//					InvStat stat = stats[i];
//					if (stat.amount == 0) continue;
//
//					if (stat.amount < 0)
//					{
//						t += "\n[FF0000]" + stat.amount;
//					}
//					else
//					{
//						t += "\n[00FF00]+" + stat.amount;
//					}
//
//					if (stat.modifier == InvStat.Modifier.Percent) t += "%";
//					t += " " + stat.id;
//					t += "[-]";
//				}
//
//				if (!string.IsNullOrEmpty(bi.description)) t += "\n[FF9900]" + bi.description;
//				UITooltip.ShowText(t);
//				return;
//			}
//		}
//		UITooltip.ShowText(null);
//	}

	/// <summary>
	/// Allow to move objects around via drag & drop.
	/// </summary>

//	void OnClick ()
//	{
			
		//NGUI Implementation:
//		if (mDraggedItem != null)
//		{
//			OnDrop(null);
//		}
//		else if (mItem != null)
//		{
//			mDraggedItem = Replace(null);
//			if (mDraggedItem != null) NGUITools.PlaySound(grabSound);
//			UpdateCursor();
//		}
//	}

	/// <summary>
	/// Start dragging the item.
	/// </summary>

//	void OnDrag (Vector2 delta)
//	{
//		if (mDraggedItem == null && mItem != null)
//		{
//			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
//			mDraggedItem = Replace(null);
//			NGUITools.PlaySound(grabSound);
//			//UpdateCursor();
//		}
//	}

	/// <summary>
	/// Stop dragging the item.
	/// </summary>

//	void OnDrop (GameObject go)
//	{
//		Item item = Replace(mDraggedItem);
//
//		if (mDraggedItem == item) NGUITools.PlaySound(errorSound);
//		else if (item != null) NGUITools.PlaySound(grabSound);
//		else NGUITools.PlaySound(placeSound);
//
//		mDraggedItem = item;
//		UpdateCursor();
//	}

	/// <summary>
	/// Set the cursor to the icon of the item being dragged.
	/// </summary>

//	void UpdateCursor ()
//	{
//		if (mDraggedItem != null && mDraggedItem.baseItem != null)
//		{
//			UICursor.Set(mDraggedItem.baseItem.iconAtlas, mDraggedItem.baseItem.iconName);
//		}
//		else
//		{
//			UICursor.Clear();
//		}
//	}

	/// <summary>
	/// Keep an eye on the item and update the icon when it changes.
	/// </summary>

//	void Update ()
//	{
//		
//		
//		//WILL COME BACK TO THIS CODE TO SEE IF IT CAN SERVE US
//		InvGameItem i = observedItem;
//
//		if (mItem != i)
//		{
//			mItem = i;
//
//			InvBaseItem baseItem = (i != null) ? i.baseItem : null;
//
//			if (label != null)
//			{
//				string itemName = (i != null) ? i.name : null;
//				if (string.IsNullOrEmpty(mText)) mText = label.text;
//				label.text = (itemName != null) ? itemName : mText;
//			}
//			
//			if (icon != null)
//			{
//				if (baseItem == null || baseItem.iconAtlas == null)
//				{
//					icon.enabled = false;
//				}
//				else
//				{
//					icon.atlas = baseItem.iconAtlas;
//					icon.spriteName = baseItem.iconName;
//					icon.enabled = true;
//					icon.MakePixelPerfect();
//				}
//			}
//
//			if (background != null)
//			{
//				background.color = (i != null) ? i.color : Color.white;
//			}
//		}
//	}
	
	//Pasted from UIStorageSlot - bastardizing my own implementation of the obsolete abstract methods
	public ItemStorage storage;
	public int slot = 0;

//	public Item observedItem
//	{
//		get
//		{
//			return (storage != null) ? storage.GetItem(slot) : null;
//		}
//	}

	/// <summary>
	/// Replace the observed item with the specified value. Should return the item that was replaced.
	/// </summary>

//	public Item Replace (Item item)
//	{
//		return (storage != null) ? storage.Replace(slot, item) : item;
//	}
	
}