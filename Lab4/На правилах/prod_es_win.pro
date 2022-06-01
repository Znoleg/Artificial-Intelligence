/*****************************************************************************

		Copyright (c) НовГУ

 Project:  PROD_ES_WIN
 FileName: PROD_ES_WIN.PRO
 Purpose: No description
 Written by: Михайлов Д.В.,  Мирошкин О.А.
 Comments:
******************************************************************************/

include "prod_es_win.inc"
include "prod_es_win.con"
include "hlptopic.con"

%BEGIN_WIN Task Window
/***************************************************************************
		Event handling for Task Window
***************************************************************************/

predicates

  task_win_eh : EHANDLER
     do_consulting
     clear
     ask(symbol,symbol)
     remember(symbol,symbol,integer)
     cloth_is(symbol)
     it_is(symbol)
     positive(symbol,symbol)
     negative(symbol,symbol)

constants

%BEGIN Task Window, CreateParms, 21:30:56-22.12.2007, Code automatically updated!
  task_win_Flags = [wsf_SizeBorder,wsf_TitleBar,wsf_Close,wsf_Maximize,wsf_Minimize,wsf_ClipSiblings]
  task_win_Menu  = res_menu(idr_task_menu)
  task_win_Title = "Экспертная система, базирующаяся на правилах"
  task_win_Help  = idh_contents
%END Task Window, CreateParms

clauses

/* Консультация */

     do_consulting:-
           cloth_is(X),!,
           concat("Ткань : ",X,Temp),
           concat(Temp,".",Result),
           dlg_Note("Экспертное заключение : ",Result),
           clear.

     do_consulting:-
           dlg_Error("Информация об интересующей вас ткани отсутствует в БЗ."),
           clear.

     ask(X,Y):-
           concat("Вопрос : ",X,Temp),
           concat(Temp," ",Temp1),
           concat(Temp1,Y,Temp2),
           concat(Temp2,"?",Quest),
           Reply1=dlg_Ask("Консультация",Quest,["Да","Нет"]),
           Reply=Reply1+1,
           remember(X,Y,Reply).

/* Механизм вывода экспертного заключения */

     positive(X,Y):-
           xpositive(X,Y),!.

     positive(X,Y):-
           not(negative(X,Y)),!,ask(X,Y).

     negative(X,Y):-
           xnegative(X,Y),!.

     remember(X,Y,1):-!,
           assertz(xpositive(X,Y)).

     remember(X,Y,2):-!,
           assertz(xnegative(X,Y)),fail.

/* Продукционные правила */

/*

          positive("обладает", "антиаллергенность"),
          positive("имеет", "хорошая прочность"),
          positive("обладает", "нерастягиваемость"),
          positive("обладает", "высокое качество"),
          positive("обладает", "дороговизна"),
          positive("обладает", "дешевизна"),
          positive("имеет", "хорошая воздухопроницаемость"),
          positive("имеет", "хорошая терморегуляция"),
          positive("имеет", "хорошая влагопоглощаемость"),

          */

     cloth_is("Хлопок"):-
           it_is("натуральный"),
           positive("обладает", "высокое качество"),
           positive("обладает", "дороговизна"),
           positive("имеет", "хорошая прочность"),!.

     cloth_is("Лён"):-
           it_is("натуральный"),
           positive("обладает", "высокое качество"),
           positive("обладает", "дороговизна"),
           positive("имеет", "хорошая воздухопроницаемость"),!.

     cloth_is("Шерсть"):-
           it_is("натуральный"),
           positive("обладает", "высокое качество"),
           positive("обладает", "дороговизна"),
           positive("обладает", "антиаллергенность"),!.

     cloth_is("Бамбук"):-
           it_is("искусственный"),
           positive("обладает", "дешевизна"),
           positive("имеет", "хорошая воздухопроницаемость"),
           positive("имеет", "хорошая прочность"),!.

     cloth_is("Ацетат"):-
           it_is("искусственный"),
           positive("обладает", "дешевизна"),
           positive("имеет", "хорошая воздухопроницаемость"),
           positive("имеет", "хорошая терморегуляция"),!.

     cloth_is("Акрил"):-
           it_is("синтетический"),
           positive("обладает", "дешевизна"),
           positive("имеет", "хорошая терморегуляция"),
           positive("обладает", "антиаллергенность"),!.

     it_is("натуральный"):-
           positive("тип сырья","натуральный"),!.

     it_is("искусственный"):-
           positive("тип сырья","искусственный"),!.

    it_is("синтетический"):-
           positive("тип сырья","синтетический"),!.

/* Уничтожение в базе данных всех ответов yes (да) и no (нет) */

     clear:-retract(xpositive(_,_)),retract(xnegative(_,_)),fail,!.
     clear.

%BEGIN Task Window, e_Create
  task_win_eh(_Win,e_Create(_),0):-!,
%BEGIN Task Window, InitControls, 21:30:56-22.12.2007, Code automatically updated!
%END Task Window, InitControls
%BEGIN Task Window, ToolbarCreate, 21:30:56-22.12.2007, Code automatically updated!
	tb_project_toolbar_Create(_Win),
	tb_help_line_Create(_Win),
%END Task Window, ToolbarCreate
ifdef use_message
	msg_Create(100),
enddef
	!.
%END Task Window, e_Create

%MARK Task Window, new events

%BEGIN Task Window, id_file
  task_win_eh(_Win,e_Menu(id_file,_ShiftCtlAlt),0):-!,
	do_consulting,!.
	
%END Task Window, id_file

%BEGIN Task Window, id_help_contents
  task_win_eh(_Win,e_Menu(id_help_contents,_ShiftCtlAlt),0):-!,
  	vpi_ShowHelp("prod_es_win.hlp"),
	!.
%END Task Window, id_help_contents

%BEGIN Task Window, id_help_about
  task_win_eh(Win,e_Menu(id_help_about,_ShiftCtlAlt),0):-!,
	dlg_about_dialog_Create(Win),
	!.
%END Task Window, id_help_about

%BEGIN Task Window, id_file_exit
  task_win_eh(Win,e_Menu(id_file_exit,_ShiftCtlAlt),0):-!,
  	win_Destroy(Win),
	!.
%END Task Window, id_file_exit

%BEGIN Task Window, e_Size
  task_win_eh(_Win,e_Size(_Width,_Height),0):-!,
ifdef use_tbar
	toolbar_Resize(_Win),
enddef
ifdef use_message
	msg_Resize(_Win),
enddef
	!.
%END Task Window, e_Size

%END_WIN Task Window

/***************************************************************************
		Invoking on-line Help
***************************************************************************/

  project_ShowHelpContext(HelpTopic):-
  	vpi_ShowHelpContext("prod_es_win.hlp",HelpTopic).

/***************************************************************************
			Main Goal
***************************************************************************/

goal

ifdef use_mdi
  vpi_SetAttrVal(attr_win_mdi,b_true),
enddef
ifdef ws_win
  ifdef use_3dctrl
    vpi_SetAttrVal(attr_win_3dcontrols,b_true),
  enddef
enddef  
  vpi_Init(task_win_Flags,task_win_eh,task_win_Menu,"prod_es_win",task_win_Title).

%BEGIN_TLB Project toolbar, 17:54:02-21.12.2007, Code automatically updated!
/**************************************************************************
	Creation of toolbar: Project toolbar
**************************************************************************/

clauses

  tb_project_toolbar_Create(_Parent):-
ifdef use_tbar
	toolbar_create(tb_top,0xC0C0C0,_Parent,
		[tb_ctrl(id_file_new,pushb,idb_new_up,idb_new_dn,idb_new_up,"New;New file",1,1),
		 tb_ctrl(id_file_open,pushb,idb_open_up,idb_open_dn,idb_open_up,"Open;Open file",1,1),
		 tb_ctrl(id_file_save,pushb,idb_save_up,idb_save_dn,idb_save_up,"Save;File save",1,1),
		 separator,
		 tb_ctrl(id_edit_undo,pushb,idb_undo_up,idb_undo_dn,idb_undo_up,"Undo;Undo",1,1),
		 tb_ctrl(id_edit_redo,pushb,idb_redo_up,idb_redo_dn,idb_redo_up,"Redo;Redo",1,1),
		 separator,
		 tb_ctrl(id_edit_cut,pushb,idb_cut_up,idb_cut_dn,idb_cut_up,"Cut;Cut to clipboard",1,1),
		 tb_ctrl(id_edit_copy,pushb,idb_copy_up,idb_copy_dn,idb_copy_up,"Copy;Copy to clipboard",1,1),
		 tb_ctrl(id_edit_paste,pushb,idb_paste_up,idb_paste_dn,idb_paste_up,"Paste;Paste from clipboard",1,1),
		 separator,
		 separator,
		 tb_ctrl(id_help_contents,pushb,idb_help_up,idb_help_down,idb_help_up,"Help;Help",1,1)]),
enddef
	true.
%END_TLB Project toolbar

%BEGIN_TLB Help line, 17:54:02-21.12.2007, Code automatically updated!
/**************************************************************************
	Creation of toolbar: Help line
**************************************************************************/

clauses

  tb_help_line_Create(_Parent):-
ifdef use_tbar
	toolbar_create(tb_bottom,0xC0C0C0,_Parent,
		[tb_text(idt_help_line,tb_context,452,0,4,10,0x0,"")]),
enddef
	true.
%END_TLB Help line

%BEGIN_DLG About dialog
/**************************************************************************
	Creation and event handling for dialog: About dialog
**************************************************************************/

constants

%BEGIN About dialog, CreateParms, 14:41:14-18.4.2022, Code automatically updated!
  dlg_about_dialog_ResID = idd_dlg_about
  dlg_about_dialog_DlgType = wd_Modal
  dlg_about_dialog_Help = idh_contents
%END About dialog, CreateParms

predicates

  dlg_about_dialog_eh : EHANDLER

clauses

  dlg_about_dialog_Create(Parent):-
	win_CreateResDialog(Parent,dlg_about_dialog_DlgType,dlg_about_dialog_ResID,dlg_about_dialog_eh,0).

%BEGIN About dialog, idc_ok _CtlInfo
  dlg_about_dialog_eh(_Win,e_Control(idc_ok,_CtrlType,_CtrlWin,_CtrlInfo),0):-!,
	win_Destroy(_Win),
	!.
%END About dialog, idc_ok _CtlInfo
%MARK About dialog, new events

  dlg_about_dialog_eh(_,_,_):-!,fail.

%END_DLG About dialog

