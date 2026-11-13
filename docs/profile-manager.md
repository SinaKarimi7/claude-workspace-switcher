# Profile manager design

Keep profile management in the Windows launcher. People should be able to identify and open either account in one click. A profile name is a user-chosen label. Sign-in happens inside Claude.

## Proposed main window

```text
Profiles                                + Add profile

  [A]  Account A       Main             Open    Edit
  [B]  Account B                        Open    Edit
```

Use a single column of rows with a shared action alignment. Use a letter tile derived from the label, a visible Main marker, and the full profile name as the accessible name. Keep the main profile first. Let rows grow with the system font and scroll vertically when needed.

## Interactions

| Action | Interface | Result |
| --- | --- | --- |
| Create | Add profile opens a named field, a Make main checkbox, and Create and Cancel buttons. Show blank or duplicate name errors beneath the field. | Save a stable ID and focus the new row. Open starts Claude with that profile’s data directory. |
| Read | Display saved names and the main marker at startup. | Enter on a focused Open button launches that row. A launch failure identifies the profile and leaves the list open. |
| Update | Edit opens the same form with the current name selected. | Rename or make main without changing the ID, directory, or existing session data. Cancel and Escape discard edits. |
| Delete | Place Remove profile in Edit. Confirm with the profile name and explain that Claude data stays on disk. | Remove only the launcher entry. Promote the next profile when removing the main one. Disable removal of the final profile and explain why. |

## Implementation and acceptance

1. Reuse the current registry XML, profile IDs, legacy paths, and save operation. Verify that an existing configuration opens with the same names, main profile, and launch paths.
2. Build the list and shared editor with native WinForms layout containers, system colors, visible keyboard focus, and content-sized buttons. Keep letter tiles decorative and expose action names such as “Open Account B” to screen readers.
3. Exercise create, reopen after restart, rename, promote, cancel, and remove using two test profiles. Verify that duplicates differing only by case are rejected and failed writes preserve the saved list.
4. Check long names, keyboard-only navigation, high contrast, and 100%, 150%, and 200% display scaling. Confirm that every button label fits. Test callback behavior separately with two real accounts before making any profile-specific sign-in promise.
