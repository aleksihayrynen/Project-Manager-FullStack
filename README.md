# 🗂️ Project Manager

### A full-stack project planning and management application built with .NET MVC

<br/>
<img width="1892" height="800" alt="image" src="https://github.com/user-attachments/assets/7410867e-5138-4f38-b895-603925153d67" />



---

## 📑 Table of Contents
- [📌 Project Overview](#project-overview)
- [✨ Key Features](#key-features)
- [🏗️ System Architecture](#system-architecture)
- [🛠️ Technologies Used](#technologies-used)
- [🗄️ Database Design](#database-design)
- [🚀 Application Functionality](#application-functionality)
- [☁️ Deployment](#deployment)
- [⚙️ Setup and Installation](#setup-and-installation)
- [🔮 Future Improvements](#future-improvements)
- [🖼️ Screenshots](#screenshots)
- [✅ Conclusion](#conclusion)

---

## 📌 Project Overview

The **Project Manager** application allows users to create and manage **groups, projects, and tasks** in a structured and permission-controlled environment.

Each user can belong to multiple groups and manage projects with **role-based access**, allowing specific rights to be assigned per user and per project. Within projects, users can create tasks, assign them to team members, and track their progress.

The core ideas and design were inspired by **Jira** and **Microsoft Teams**.  
For more technical details, see the **Technical Report** included in the repository.

---

## ✨ Key Features
- Secure user authentication and identification  
- User profiles with custom images  
- Group-based project organization  
- Role-based access control  
- Project-specific task management  
- Task creation, assignment, and status tracking  

---

## 🏗️ System Architecture
The application follows the MVC (Model–View–Controller) pattern, separating business logic, user interface, and data handling to improve maintainability and scalability.

Most of the application logic is separated into a Services layer, which is injected into controllers using dependency injection. This approach keeps controllers lightweight and allows business logic to remain modular, testable, and reusable.

---

## 🛠️ Technologies Used
- ASP.NET MVC (.NET)  
- Authentication & Authorization  
- Entity Framework  
- MongoDB  
- Argon2 password encryption  
- Blazor Pages  
- HTML, CSS, JavaScript  

---

## 🗄️ Database Design
MongoDB was used as the database solution for this project. Its non-relational structure provided flexibility when designing and iterating on data models, allowing the schema to evolve throughout development.

---

## 🔐 Enncryption
Argon2 was used to encrypt the user passwords with specific settiings from the `appsettings.json` file. Each user also has an unique salt value stored in the database.

---

## 🚀 Application Functionality
Users can:
- Register and log in securely  
- Create or join groups  
- Manage projects within groups  
- Create, assign, and track tasks  

All actions are governed by role-based permissions to ensure proper access control.

---

## ☁️ Deployment
The application was deployed using **Microsoft Azure** and secured with **SSL** under the domain **Planmelon.fi**.

⚠️ The website is currently offline.

---

## ⚙️ Setup and Installation

> ⚠️ The repository does not include `appsettings.json` files due to sensitive configuration data.

1. Clone the repository  
2. Add the required `appsettings.json` files  
3. Configure database connection settings in `appsettings.json`
4. Configure Argon2 encryption settings in `appsettings.json`
5. Build and deploy the application  

---

## 🔮 Future Improvements
- Real-time notifications  
- Analytics and reporting features  
- In-project messaging system  

---

## 🖼️ Screenshots

<table>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/470b6dec-6a0e-4f4c-98f9-612ab01b8115" width="100%" />
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/a9a984e3-d13d-46f6-ae8c-73822e8f75af" width="100%" />
    </td>
  </tr>
  <tr>
    <td>
      <img src="https://github.com/user-attachments/assets/ba26fbb8-a494-4479-b7f2-f7e2cc3dffb7" width="100%" />
    </td>
    <td>
      <img src="https://github.com/user-attachments/assets/15908f9c-7380-469e-a3c6-e533260b21fe" width="100%" />
    </td>
  </tr>
</table>



---

- **Project Created By**: Aleksi Häyrynen
 

