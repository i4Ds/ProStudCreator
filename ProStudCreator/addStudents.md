## Overview
This document outlines the process of adding `EventoID` from one dataset to another, generating SQL `UPDATE` statements, and applying them to the database.

---

## Step 1: Merging `EventoID` into the Main Dataset

### **1.1 Understanding the Data**
- **`File.xlsx`**: Contains student details including `EventoID`.
- **`P56-Zuteilung-semester.xlsx`**: Contains project assignments but lacks `EventoID`.

### **1.2 Steps to Merge `EventoID`**
1. Load both Excel files into **Pandas DataFrames**.
2. Merge the datasets based on the common column `Email`.
3. Save the updated dataset with `EventoID` back to an Excel file.

### **1.3 Code Implementation**
```python
import pandas as pd

# Load Excel files
file1 = 'P56-Zuteilung-semester.xlsx'
file2 = 'File.xlsx'

p56_df = pd.read_excel(file1, sheet_name='Project Distribution')
v4_df = pd.read_excel(file2, sheet_name='Project Distribution')

# Merge using Email as key
merged_df = pd.merge(p56_df, v4_df[['Email', 'EventoID']], on='Email', how='left')

# Save updated file
merged_df.to_excel('Merged_P56-Zuteilung-semester.xlsx', index=False, sheet_name='Updated Project Distribution')

```  

## Step 2: Generating SQL `UPDATE` Statements
              

### **2.1 Steps to Generate SQL Statements**

1. Load the **merged Excel file**.
2. Group students by project name.
3. Apply mapping rules to determine `LogStudyCourse`, `LogProjectTypeID`, and `LogProjectDuration`.
4. Format and generate **SQL `UPDATE` statements**.
6. Save the SQL script to a `.sql` file.

### **2.2 Code Implementation**

```python
def escape_single_quotes(text):
    return text.replace("'", "''")

def generate_sql_updates(df):
    updates = []
    processed_projects = set()

    study_course_mapping = {"I/ICom": 1, "DS": 2}
    project_type_mapping = {"IP5": 1, "IP6": 2}
    project_duration_mapping = {("IP5", "17 Wochen"): 1, ("IP6", "26 Wochen"): 1, ("IP5", "26 Wochen"): 2}

    for _, row in df.iterrows():
        project_name = escape_single_quotes(row['Projekt'])
        if project_name in processed_projects:
            continue

        # Get all students for the project
        project_rows = df[df['Projekt'] == row['Projekt']]
        students = []
        for _, proj_row in project_rows.iterrows():
            students.append({
                "EventoID": proj_row['EventoID'],
                "FirstName": proj_row['Vorname'],
                "LastName": proj_row['Name'],
                "Email": proj_row['Email'],
                "StudyCourse": study_course_mapping.get(proj_row['Studiengang'], 1),
                "ProjectType": project_type_mapping.get(proj_row['Projekttyp'], 1),
                "ProjectDuration": project_duration_mapping.get((proj_row['Projekttyp'], proj_row['Projektdauer']), 1)
            })

        # Generate SQL statement based on the number of students
        if len(students) == 1:
            s = students[0]
            update = f"""
            UPDATE Projects
            SET LogProjectTypeID = {s['ProjectType']},
                LogProjectDuration = {s['ProjectDuration']},
                LogStudyCourse = {s['StudyCourse']},
                LogStudent1Evento = '{s['EventoID']}',
                LogStudent1FirstName = '{s['FirstName']}',
                LogStudent1LastName = '{s['LastName']}',
                LogStudent1Mail = '{s['Email']}',
                LogStudyCourseStudent1 = {s['StudyCourse']}
            WHERE SemesterId = 81 AND Name = '{project_name}';
            """
        elif len(students) == 2:
            s1, s2 = students
            update = f"""
            UPDATE Projects
            SET LogProjectTypeID = {s1['ProjectType']},
                LogProjectDuration = {s1['ProjectDuration']},
                LogStudyCourse = {s1['StudyCourse']},
                LogStudent1Evento = '{s1['EventoID']}',
                LogStudent1FirstName = '{s1['FirstName']}',
                LogStudent1LastName = '{s1['LastName']}',
                LogStudent1Mail = '{s1['Email']}',
                LogStudyCourseStudent1 = {s1['StudyCourse']},
                LogStudent2Evento = '{s2['EventoID']}',
                LogStudent2FirstName = '{s2['FirstName']}',
                LogStudent2LastName = '{s2['LastName']}',
                LogStudent2Mail = '{s2['Email']}',
                LogStudyCourseStudent2 = {s2['StudyCourse']}
            WHERE SemesterId = 81 AND Name = '{project_name}';
            """
        updates.append(update.strip())
        processed_projects.add(project_name)
    return updates
```


## Step 3: Applying SQL Updates to the Database

### **3.1 Running the SQL Script**
1. Open a **SQL client** (e.g., MySQL Workbench, SQL Server Management Studio, or PostgreSQL).
2. Load the generated `Projects_Update_Statements.sql` file.
3. Run the script to apply updates:




## How to Start the Projects

Once all data is correctly updated in the database, you need to start the project by changing its **state**. This ensures that the project is active and ready for execution.

### **4.1 Running the SQL Script**
To start all projects for **Semester 25FS** (which corresponds to `SemesterId = 81`), run the following SQL statement:

```sql
UPDATE Projects
SET State = 4
WHERE SemesterId = 81;
```


## Conclusion
This document outlines how to merge EventoID, generate SQL statements, and update the database.
if you have any question about this, please let me know: delberin.ali@fhnw.ch