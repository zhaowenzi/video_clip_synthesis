   M S B A M L   `   `   `  ����W  SVlc.DotNet.Forms, Version=3.0.1.0, Culture=neutral, PublicKeyToken=84529da31f4eb963M8clr-namespace:Vlc.DotNet.Forms;assembly=Vlc.DotNet.FormsVlc.DotNet.Forms  5      ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration 5   	    素材合成   素材合成.ScreenWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   O
VlcWinform8clr-namespace:Vlc.DotNet.Forms;assembly=Vlc.DotNet.Forms     5      lWindowsFormsHostOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration     5   	   -   5          �Name$  Window��5   	    � WindowStyle$
 None=�5	   	    � 
ResizeMode$ NoResize=�5
   	   
������  � AllowsTransparency .   � Title$ 
PayWindows��5   	   $	��450��6   $	��700��6)    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ /Styles/Base.xaml?�6%   �� 5      $ /Styles/CommonStyles.xaml?�6%   %    5      .��5      ��  �� CornerRadius$ 6��6   
������ ���#C1C1C1$	��0.5q�6N   ._�5   
   � 
������ 	 � RowDefinitions	 5      �� 5      $��40 �6    6.   �� 5      6"   .+�5       �� -   5        $ 6,6,0,0��5       $��40��6,   $��0q�6i   ��5!      
� 5"      
 
� GradientOrigin$
 0.5,0.5&�6*    
� RadiusX$ 1Q�6C    
� RadiusY$ 1Q�6O   .��5#      �  � Color$
 #111��6'    � Offset$ 0Q�64   � 5$      $
 #333��6'   $	 0.5Q�64   � 5%      $
 #111��6'   $ 0Q�64   5&      ._�5(      � $��0��6    � ColumnDefinitions 5)      �� 5*      $	��40* �6+   �� 5+      $	��60* �6+   .+�5-      �� $��Center=�6    $��16(�6;   $��Regular"�6I   
�������$��10,0,0,0q�6q   分屏6�   �� 5.      -   5        $��1��5.      $��/img/ic_关闭_白.png��6,   $��16��6F   $��16��6Q    � Cursor$
 Hand��5/      $��Right=�50      $��0,0,10,0q�68   6J   51      52      �� 54      ���#CBCBCB$��0q�6+   $ 0,0,6,6��6?   
�������$��1��6i   ._�55      � 
������ .+�56      7  1System.Windows.Forms.Integration.WindowsFormsHost  -   5          Name$ winformhost��56   8   $��5q�6M      Child. 57      !   Vlc.DotNet.Forms.VlcControl  -   5        	VlcPlayer��[ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"  System.Windows.Forms.Control   Dock$
 Fill=�57   E   6�   58      59      5:      5;      5<      5=      