# 📚 Documentation

This folder contains the complete documentation for the **Student Registration System**.

---

## 📖 Contents

| #   | File                                                   | Description                                  |
| --- | ------------------------------------------------------ | -------------------------------------------- |
| 1   | [`01-ERD.md`](01-ERD.md)                               | Entity Relationship Diagram (Mermaid format) |
| 2   | [`01-ERD.puml`](01-ERD.puml)                           | ERD in PlantUML format                       |
| 3   | [`02-ClassDiagram.md`](02-ClassDiagram.md)             | Class Diagram (Mermaid format)               |
| 4   | [`02-ClassDiagram.puml`](02-ClassDiagram.puml)         | Class Diagram in PlantUML format             |
| 5   | [`PROJECT_DOCUMENTATION.md`](PROJECT_DOCUMENTATION.md) | Full technical documentation                 |
| 6   | `screenshots/`                                         | UI screenshots                               |

---

## 🎨 How to View

### On GitHub

The `.md` files with Mermaid will render automatically.

### In VS Code

Install extensions:

- **Markdown Preview Mermaid Support** (`bierner.markdown-mermaid`)
- **PlantUML** (`jebbs.plantuml`)

Then press `Ctrl + Shift + V` to preview.

### Generate Images

**Mermaid:** Use https://mermaid.live/ → Paste → Export PNG/SVG

**PlantUML:** Use https://www.plantuml.com/plantuml/uml/ → Paste → Download PNG/SVG

---

## 📐 Diagrams Overview

### ERD (Entity Relationship Diagram)

Shows **database schema**:

- 17 tables
- Primary keys (PK)
- Foreign keys (FK)
- Unique keys (UK)
- Relationships and cardinality
- Constraints

### Class Diagram

Shows **C# classes**:

- 17 entities
- Properties
- Navigation properties
- Methods (e.g., `HasAvailableSeats()`)
- Inheritance from `BaseEntity`

---

## 🖼️ Screenshots

Place UI screenshots in `screenshots/` folder:

| File               | Description                     |
| ------------------ | ------------------------------- |
| `login.png`        | Login page                      |
| `dashboard.png`    | Student dashboard               |
| `registration.png` | Online registration page        |
| `schedule.png`     | Weekly schedule                 |
| `print.png`        | Print-friendly view             |
| `admin.png`        | Admin dashboard                 |
| `conflict.png`     | Schedule conflict demonstration |
| `18hour.png`       | 18-hour limit demonstration     |
