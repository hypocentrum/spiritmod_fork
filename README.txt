Permanent buff fix

Your live dump showed:
- SkillDisplays_C has Conviction with Duration=0
- StatusDisplays_C has CombatMounted with negative duration
- StatusDisplays_C has SpearQuicken and Endure with long durations

So Conviction Aura and Gryphon Riding should be treated as "permanent until death":
- active if their marker exists
- remainingSeconds = -1
- do not recast while active
- recast only if missing after death/disconnect/session reset

Files:
1. BuffMaintenanceRuntimeRules.cs
   Add this new file to the SpiritMod project.

2. CombatService_permanent_buff_methods_REPLACE.txt
   Replace CombatService.IsPermanentBuff and CombatService.TryGetBuffRemainingSeconds with these versions.
   Add all helper methods into CombatService class.

3. UNDO_previous_exclusion_note.txt
   If you removed Conviction Aura from Buff Maintenance before, undo that exclusion.
