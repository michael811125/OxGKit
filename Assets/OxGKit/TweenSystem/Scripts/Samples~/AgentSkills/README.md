# OxGKit.TweenSystem AI Agent Skill

An Agent Skill for AI coding agents (Claude Code, OpenAI Codex CLI, Cursor, Copilot, etc.) that teaches the agent how to use **OxGKit.TweenSystem** — designer-authorable DOTween Pro tween components: DoTweenAnim tracks with editor preview and the DoTweenAnimEvent group player.

The skill follows the Agent Skills convention (a folder with a `SKILL.md` entry file: YAML frontmatter + instructions). API usage is verified against the OxGKit.TweenSystem source of this package version.

## What is included

| Skill | Purpose |
|---|---|
| `oxgkit-unity-skill` | OxGKit.TweenSystem usage: DoTweenAnim track authoring, DoTweenAnimEvent playback with callbacks, DOTween assembly setup, and the GUID fix. |

Every OxGKit system ships its own `oxgkit-unity-skill` folder from its Package Manager Samples — the folder/skill name is the same for all systems, while the content is system-specific (see "Using multiple OxGKit systems" below).

## How to install

1. Import this sample through **Package Manager > OxGKit.TweenSystem > Samples > AI Agent Skills**.
   The files are copied to `Assets/Samples/OxGKit.TweenSystem/<version>/AI Agent Skills/`.
2. Copy (or symlink) the skill folder to wherever your agent discovers skills:

   | Agent | Location |
   |---|---|
   | Claude Code | `<project>/.claude/skills/oxgkit-unity-skill/` (or `~/.claude/skills/` for all projects) |
   | OpenAI Codex CLI | `<project>/.agents/skills/oxgkit-unity-skill/` (or `~/.codex/skills/`) |
   | Other agents | Reference the folder from your agent context file (`AGENTS.md`, `CLAUDE.md`, `.cursorrules`, ...), e.g. "Read Assets/Samples/.../oxgkit-unity-skill/SKILL.md before working with OxGKit.TweenSystem APIs." |

3. Optionally delete the imported copy under `Assets/Samples/` afterwards — the skill files are plain Markdown and do not need to live inside the Unity project once copied. (Keeping them under `Assets/` is harmless; they import as TextAssets.)

## Using multiple OxGKit systems

All OxGKit system skills share the folder/skill name `oxgkit-unity-skill` so that each system module can ship one independently. If your project uses several OxGKit systems, merge them into ONE skill folder:

1. Keep a single `SKILL.md` frontmatter block (`name: oxgkit-unity-skill` plus a combined `description:` covering the systems you use).
2. Concatenate the bodies — each system's body starts at its own `# OxGKit.<System> Unity Skill` heading, so bodies can be appended as-is.

## Notes

- The skill is written in English so it works well as agent context in any locale; the agent should reply in the user's language.
- API signatures were verified against OxGKit.TweenSystem v1.0.4. Newer versions may differ — the skill instructs the agent to re-verify against the installed package source.
- The OxGFrame framework provides its own `oxgframe-unity-skill` separately (https://github.com/michael811125/OxGFrame).
