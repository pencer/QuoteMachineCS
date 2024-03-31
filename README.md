# QuoteMachineCS
Add/Remove quote ("> ") to/from text in clipboard.
The current hotkey settings:

    Ctrl+F8: Remove Quote
    Ctrl+F9: Add Quote
    Ctrl+F10: Add Quote in Markdown style (three back-quotes)
    Ctrl+F11: Add Quote in Markdown style then append to previously created contents by Ctrl+F10 or Ctrl+F11
    Ctrl+F12: Convert UNC to File URI

ex.) Add quote

Before in clipboard:

    aaa
    bbb


After in clipboard:

    > aaa
    > bbb

ex.) Add quote in Markdown style

Before in clipboard:

    aaa
    bbb


After in clipboard:

    ```
    aaa
    bbb
    ```

The created contents is stored as an internal varialble and recalled in Ctrl+F11.

This application is running in task tray. You can right-click the icon in the task tray to exit.

Platform: Windows10.

Thanks.
