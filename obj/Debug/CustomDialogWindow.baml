   M S B A M L   `   `   `  ����  LoudSong!    LoudSong.CustomDialogWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$   Setting Song's Information��5   	   $	��250��6,   $	��580��69   
����İ� � WindowStyle$
 None=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   .��5
      � -   5         �Name$ dialogWindowGrid��5
       � RowDefinitions 5   
   �� 5      $
��0.4* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   � -   5         �Name$ panelButtons��5      $��0��6*   $��0��67   $��2��6G   $��Right=�6[   $	��Top=�6w   .+�5      �� -   5         ��Name$ txtBlockMovingWindow��5      $	��500��66   6B   A� 5      -   5         A�Name$ btnMin��5      
����iA�$��20��67   $��20��6C   $��0,3,5,3q�6N   6_   A� 5      -   5        $ btnClose��5      
����p��$��20��6=   $��20��6I   $��0,3,5,3q�6T   6e   5      �� 5"   
   -   5        	 ��Name$	 lblCustomDialog��5"      $��1��6)   $��0��66   $��2��6F   $��10q�6Z   $
��Left=�6g   $	��Top=�6�   �� 5%   
   -   5        $��txtBoxCustomDialog��5%      $��2��6,   $��0��69   $��2��6I   $��3,3,0,0q�6]   $��0,0,0,5q�6o   $��Center=�6�   $��Center=�5&      $��25��6-   $	��200��69   
 �� 	MaxLength$

 4000��6E    �� TextWrapping$ NoWrap=�6V   �� 5)   
   -   5        $��btnOkCustomDialog��5)      $��3��6*   $��0��67   $��5q�6G   $��0,0,0,5q�6S   $��OK��6d   $��Center=�6q   $��Center=�6�   $	��100��6�   6�   �� 5*   
   -	   5        $��btnCancelCustomDialog��5*      $��3��6.   $��1��6;   $��5q�6K   $��0,0,0,5q�6W   $��Cancel��6h   $��Center=�6y   $��Center=�5+      $	��100��6,   68   5-      5/      