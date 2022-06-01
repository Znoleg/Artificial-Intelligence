/*****************************************************************************

		Copyright (c) НовГУ

 Project:  logic_es_win
 FileName: LOGIC_ES_WIN.INC
 Purpose:  Project's global declarations
 Written by: Михайлов Д.В.
 Comments:
*****************************************************************************/

constants
%BEGIN_CONST Project configuration constants, Code automatically updated!
  use_mdi = 1
  use_3dctrl = 1
  use_tbar = 1
  use_dlgpack = 1
  use_editor = 1
  use_tree = 1
  use_message = 1
%END_CONST

include "vpi.con"
include "vpi.dom"
include "vpi.pre"


/*****************************************************************************
			User's Declarations
*****************************************************************************/
global domains
  DB_SELECTOR = browselist_db	% For treebrowser tool
  FILE = fileselector1; fileselector2 % To be edited
     rule_number,cond_number=integer
     category,condition,breed_of_dog=symbol
     conditions=cond_number*     
     history=rule_number*
     word_list=category*
     char_list=char*

include "logic_es_win.pre"

include "vpitools.dom"
include "vpitools.pre"
