﻿/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.plugins.setLang( 'a11yhelp', 'zh', {
	title: '輔助工具指南',
	contents: '說明內容。若要關閉此對話框請按「ESC」。',
	legend: [
		{
		name: '一般',
		items: [
			{
			name: '編輯器工具列',
			legend: '請按「${toolbarFocus}」以瀏覽工具列。\r\n利用「TAB」或「SHIFT+TAB」以便移動到下一個或前一個工具列群組。\r\n利用「→」或「←」以便移動到下一個或前一個工具列按鈕。\r\n請按下「空白鍵」或「ENTER」鍵啟動工具列按鈕。'
		},

			{
			name: '編輯器對話方塊',
			legend: '在對話框中，請按 TAB 鍵以便移動到下個欄位，請按 SHIFT + TAB 以便移動到前個欄位；請按 ENTER 以提交對話框資料，或按下 ESC 取消對話框。\r\n若是有多個頁框的對話框，請按 ALT + F10 以移動到頁框列表，並以 TAB 或是 → 方向鍵移動到下個頁框。以 SHIFT + TAB 或是 ← 方向鍵移動到前個頁框。按下 空白鍵 或是 ENTER 以選取頁框。'
		},

			{
			name: '編輯器內容功能表',
			legend: '請按下「${contextMenu}」或是「應用程式鍵」以開啟內容選單。以「TAB」或是「↓」鍵移動到下一個選單選項。以「SHIFT + TAB」或是「↑」鍵移動到上一個選單選項。按下「空白鍵」或是「ENTER」鍵以選取選單選項。以「空白鍵」或「ENTER」或「→」開啟目前選項之子選單。以「ESC」或「←」回到父選單。以「ESC」鍵關閉內容選單」。'
		},

			{
			name: '編輯器清單方塊',
			legend: '在列表中，請利用 TAB  或  ↓ 方向鍵以移動到下一個項目；或利用 SHIFT + TAB 或 ↑ 方向鍵移動到前一個項目。請按下 空白鍵 或是 ENTER 以選取項目。請按 ESC 關閉列表。'
		},

			{
			name: '編輯器元件路徑工具列',
			legend: '請按「${elementsPathFocus}」以瀏覽元素路徑工具列。\r\n利用「TAB」或「→」以便移動到下一個元素按鈕。\r\n利用「SHIFT+TAB」或「←」以便移動到前一個元素按鈕。\r\n請按下「空白鍵」或「ENTER」鍵選擇編輯器中的元素。'
		}
		]
	},
		{
		name: '命令',
		items: [
			{
			name: '復原命令',
			legend: '請按下「${undo}」'
		},
			{
			name: '重複命令',
			legend: '請按下「 ${redo}」'
		},
			{
			name: '粗體命令',
			legend: '請按下「${bold}」'
		},
			{
			name: '斜體',
			legend: '請按下「${italic}」'
		},
			{
			name: '底線命令',
			legend: '請按下「${underline}」'
		},
			{
			name: '連結',
			legend: '請按下「${link}」'
		},
			{
			name: '隱藏工具列',
			legend: '請按下「${toolbarCollapse}」'
		},
			{
			name: '存取前一個焦點空間命令',
			legend: '請按下 ${accessPreviousSpace} 以存取最近但無法靠近之插字符號前的焦點空間。舉例：二個相鄰的 HR 元素。\r\n重複按鍵以存取較遠的焦點空間。'
		},
			{
			name: '存取下一個焦點空間命令',
			legend: '請按下 ${accessNextSpace} 以存取最近但無法靠近之插字符號後的焦點空間。舉例：二個相鄰的 HR 元素。\r\n重複按鍵以存取較遠的焦點空間。'
		},
			{
			name: '協助工具說明',
			legend: '請按下「${a11yHelp}」'
		}
		]
	}
	]
});
