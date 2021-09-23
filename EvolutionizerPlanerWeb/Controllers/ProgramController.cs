using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using ProgramDataAccess;


/***********************************/
// [X] Tasks can depend on other Tasks(even multiple) to be finished before they can be start
// [X] the start and end date of tasks must be set and cannot be left uninitialized
// [X] the applictaion shall prevent setting an incorrect end date that happen before the start date of a task, but
//     it is allowed to have the same start/end date
// [ ] it shall also prevent defining dependencies that are impossible, caused by overlapping dates
// [X] the removale of a program or project shall be prevented if it still contains projects or tasks beneath it
// 
// [ ] for projects, the start and end date shall be calculated on the underlying tasks (min/max)
// [ ] for programs, the start and end date shall be calculated on the underlying projects (min/max)
//
// [X] duration of tasks is the difference of the start and end date
// [ ] duration of projects is the sum of all the tasks durations in the longest task dependency chain without time gaps
/***********************************/

namespace EvolutionizerPlanerWeb.Controllers {
    public class ProgramController : ApiController {

        /// Section helper functions
        /// 
        //enum Calculation_level{ program, project, task}; 

        private int CalcDuration(   DateTime            end,
                                    DateTime            start/*,
                                    Calculation_level   level,
                                    Program             program,
                                    Projects            projects,
                                    Tasks               tasks*/) {
            if (DateTime.Compare(start, end) > 0) {
                return 0;

            } else {
                TimeSpan duration = end - start;
                return (int)duration.TotalDays;

            }

            // Mit einer Schleife vom jedem Task ausgehend pro Project die Duration aufsummieren

            // mit einer Schleife von jedem Project die untergebenen Tasks durchiterieren und
            // das Späteste und mittels spätesten und frühsten Task die duration ermitteln


            // Call SP wich calculate longest duration

            /*switch (level) {
                case Calculation_level.program:
                    return 0;
                    //break;
                case Calculation_level.project:
                    return 0;
                   // break;
                case Calculation_level.task:

                    if (DateTime.Compare(start, end) > 0) {
                        return 0;

                    } else {
                        TimeSpan duration = end - start;
                        return (int)duration.TotalDays;

                    }

                   //break;
                default:
                   return 0;
                    //break;
            }*/


        }

        private int getUTCTimestamp() {
            // outsourced for the purpose of standardization
            return DateTime.UtcNow.Millisecond;
        }

        /// Section API-Calls
        [HttpGet]
        [ActionName("ProgramsGet")]
        // returns a list of Programs from the database 
        public IEnumerable<Program> ProgramGet() { 
            using (AllProgramEntities entities = new AllProgramEntities()) {
                return entities.Program.ToList();
                //return AutoMapperBase._mapper.Map<AutoMapperBase.ProgramDto>(entities.Program);
            }
        }
        [HttpGet]
        [ActionName("ProjectsGet")]
        // returns a list of Projects from the database
        public IEnumerable<Projects> ProjectsGet() {
            using (AllProgramEntities entities = new AllProgramEntities()) {
                return entities.Projects.ToList();
            }
        }
        [HttpGet]
        [ActionName("TasksGet")]
        // returns a list of Tasks from the database
        public IEnumerable<Tasks> TasksGet() {
            using (AllProgramEntities entities = new AllProgramEntities()) {
                return entities.Tasks.AsEnumerable().ToList();
            }
        }


        [HttpGet]
        [ActionName("ProgramGetByID")]
        // returns the Program from the database with ID
        public Program ProgramGet(int id) {
            using (AllProgramEntities entities = new AllProgramEntities()) {
                return entities.Program.FirstOrDefault(e => e.ProgramID == id);
            }
        }
        [HttpGet]
        [ActionName("ProjectGetByID")]
        // returns the Project from the database with ID
        public Projects ProjectGet(int id) {
            using (AllProgramEntities entities = new AllProgramEntities()) {
                return entities.Projects.FirstOrDefault(e => e.ProjectID == id);
            }
        }
        [HttpGet]
        [ActionName("TaskGetByID")]
        // returns the Task from the database with ID
        public Tasks TasksGet(int id) {
            using (AllProgramEntities entities = new AllProgramEntities()) {
                return entities.Tasks.AsEnumerable().FirstOrDefault(e => e.TaskID == id);
            }
        }



        [HttpPost]
        [ActionName("AddNewProgram")]
        // adds one Program to the database with parameter id, name, description, startDate and endDate
        public string AddProgram(   int         id,
                                    string      name,
                                    string      description,
                                    DateTime    startDate,
                                    DateTime    endDate) {

            // checks the parameters and gives feedback 
            if (startDate.ToString().Length == 0)
                return "startDate is empty!";

            if (endDate.ToString().Length == 0)
                return "endDate is empty!";

            if (startDate.ToString().Length < 19) // 01-01-1970-00:00:00
                return "invalid startDate? : " + startDate.ToString();

            if (endDate.ToString().Length < 19)
                return "invalid endDate? : " + endDate.ToString();

            if (DateTime.Compare(startDate, endDate) > 0)
                return startDate.ToString() + " is later than " + endDate.ToString();

            if (name.Length == 0)
                return "Name is mandatory";


            using (AllProgramEntities entities = new AllProgramEntities()) {

                try { // checks whether there is an entry with the number.
                      // catches the NullReferenceException
                    if ( entities.Program.FirstOrDefault(e => e.ProgramID == id).ProgramID == id )
                        return "Program exists. Use PUT-Methode updateProgram instead for update.";
                } catch (System.NullReferenceException) {
                    entities.Program.Add(new Program() {

                        ProgramID   = id,
                        Name        = name,
                        Description = description,
                        StartDate   = startDate,
                        EndDate     = endDate,
                        status      = 0,
                        Duration    = CalcDuration(endDate, startDate),
                        Timestamp   = getUTCTimestamp(),

                    });
                    // apply the changes, check out the New Program object
                    entities.SaveChanges();
                    
                }
                return "Successful created Program-entitiy: " + name;
            }
            
        }
        [HttpPost]
        [ActionName("AddNewProject")]
        public string AddProjects(  int         id,
                                    string      name,
                                    string      description,
                                    DateTime    startDate,
                                    DateTime    endDate,
                                    int         ProgramId) {
            using (AllProgramEntities entities = new AllProgramEntities()) {

                try { // checks whether there is an entry with the number.
                      // catches the NullReferenceException
                    if (entities.Projects.FirstOrDefault(e => e.ProjectID == id).ProjectID == id)
                        return "Project exists. Use PUT-Methode updateProgram instead for update.";
                } catch (System.NullReferenceException) {
                    entities.Projects.Add(new Projects() {

                        ProjectID = id,
                        Name = name,
                        Description = description,
                        StartDate = startDate,
                        EndDate = endDate,
                        Timestamp = getUTCTimestamp(),
                        Duration = CalcDuration(endDate, startDate),
                        status = 0,
                        ProgramId = ProgramId
                    });
                    entities.SaveChanges();
                }
                return "Successful created Projects-entitiy: " + name;
            }
            
        }
        [HttpPost]
        [ActionName("AddNewTask")]
        public string AddTasks( int         id,
                                string      name,
                                string      description,
                                DateTime    startDate,
                                DateTime    endDate,
                                int         projectId,
                                int         afterme_TaskID /* muss auch leer bleiben dürfen*/) {
            using (AllProgramEntities entities = new AllProgramEntities()) {


                try { // checks whether there is an entry with the number.
                      // catches the NullReferenceException
                    if (entities.Tasks.FirstOrDefault(e => e.TaskID == id).TaskID == id)
                        return "Task exists. Use PUT-Methode updateProgram instead for update.";
                } catch (System.NullReferenceException) {
                    
                    // prüfen ob die Taskabhängigkeit möglich ist
                    entities.Tasks.Add(new Tasks() {
                        TaskID = id,
                        Name = name,
                        Description = description,
                        StartDate = startDate,
                        EndDate = endDate,
                        DepentsOn = null,
                        status      = 0,
                        Timestamp = getUTCTimestamp(),
                        Duration = CalcDuration(endDate, startDate),
                        ProjectsId = projectId,
                        AfterMe_TaskID = afterme_TaskID
                    });
                    entities.SaveChanges();
                }
                return "Successful created Tasks-entitiy: " + name;
            }
        }



        [HttpPut]
        [ActionName("updateProgram")]
        public string updateProgram(    int         id, 
                                        string      name, 
                                        string      description, 
                                        DateTime    startDate, 
                                        DateTime    endDate,
                                        byte        status) {

            using (AllProgramEntities PE = new AllProgramEntities() ) {

                try {
                    Program prog = PE.Program.Where(c => c.ProgramID == id).Single<Program>();

                    prog.ProgramID          = id;
                    prog.Name               = name;
                    prog.Description        = description;
                    prog.StartDate          = startDate;
                    prog.EndDate            = endDate;
                    prog.Duration           = CalcDuration(endDate, startDate); // // Call SP wich calculate longest duration
                    prog.Timestamp          = getUTCTimestamp();
                    prog.status             = status;

                    PE.Entry(prog).State    = System.Data.Entity.EntityState.Modified;
                    PE.SaveChanges();
                } catch (SystemException exce) {
                    if (exce is InvalidOperationException || exce is System.NullReferenceException) {
                        return "Program: " + id + " dosn't exist. Use Post-Methode AddNewProgram first.";
                    }
                }
            }            
            return "Successfull update Program-enity:" + name;
        }
        [HttpPut]
        [ActionName("updateProject")]
        public string updateProject(    int         id,
                                        string      name,
                                        string      description,
                                        DateTime    startDate,
                                        DateTime    endDate,
                                        byte      status,
                                        int         programId) {

            using (AllProgramEntities APE = new AllProgramEntities()) {
                
                try { 
                    Projects proj = APE.Projects.Where(c => c.ProjectID == id).Single<Projects>();

                    proj.ProjectID      = id;
                    proj.Name           = name;
                    proj.Description    = description;
                    proj.StartDate      = startDate;
                    proj.EndDate        = endDate;
                    proj.Timestamp      = getUTCTimestamp();
                    proj.Duration       = CalcDuration(endDate, startDate); // // Call SP wich calculate longest duration
                    proj.status         = status;
                    proj.ProgramId      = programId;

                    APE.Entry(proj).State = EntityState.Modified;
                    APE.SaveChanges();
                } catch (SystemException exce) {
                    if (exce is InvalidOperationException || exce is System.NullReferenceException) {
                        return "Project: " + id + " dosn't exist. Use Post-Methode AddNewProject first.";
                    }
                }
            }
            return "Successfull update Projects-enity:" + name;
        }
        [HttpPut]
        [ActionName("updateTask")]
        public string updateTask(   int         id,
                                    string      name,
                                    string      description,
                                    DateTime    startDate,
                                    DateTime    endDate,
                                    byte        status,
                                    int         projectId,
                                    int         afterme_TaskID) {

            using (AllProgramEntities APE = new AllProgramEntities()) {

                try {
                    Tasks tas = APE.Tasks.Where(c => c.TaskID == id).Single<Tasks>();

                    tas.TaskID = id;
                    tas.Name = name;
                    tas.Description = description;
                    tas.StartDate = startDate;
                    tas.EndDate = endDate;
                    tas.Timestamp = getUTCTimestamp();
                    tas.Duration = CalcDuration(endDate, startDate); // // Call SP wich calculate longest duration
                    tas.status = status;
                    tas.ProjectsId = projectId;

                    APE.Entry(tas).State = EntityState.Modified;
                    APE.SaveChanges();
                } catch (SystemException exce) {
                    if (exce is InvalidOperationException || exce is System.NullReferenceException) {
                        return "Task: " + id + " dosn't exist. Use Post-Methode AddNewProject first.";
                    }
                }
            }
            return "Successfull update Task-enity:" + name;
        }



        [HttpDelete]
        [ActionName("DeleteProgram")]
        public string DeleteProgram([FromUri] int id) {

            // if contains Project > 0 dont delete
            using (AllProgramEntities entities = new AllProgramEntities()) {
                var owner = entities.Program.Where( c=> c.ProgramID == id).Single<Program>();
                
                if (owner == null)
                    return "Not Found";
               
                if (owner.Projects.Count() > 0)
                    return "Program contains any Projects. Delete this Tasks first.";
                
                entities.Program.Remove(owner);
                entities.SaveChanges();
            
            }

                return "Deleted Program: " + id;
        }
        [HttpDelete]
        [ActionName("DeleteProject")]
        public string DeleteProjects([FromUri] int id) {

            using (AllProgramEntities entities = new AllProgramEntities()) {
                var owner = entities.Projects.Where(c => c.ProjectID == id).Single<Projects>();
                
                if (owner == null)
                    return "Not Found";

                if (owner.Tasks.Count() > 0)
                    return "Project contains any Tasks. Delete this Tasks first.";

                entities.Projects.Remove(owner);
                entities.SaveChanges();

            }
            return "Deleted Projects: " + id;
        }
        [HttpDelete]
        [ActionName("DeleteTask")]
        public string DeleteTasks([FromUri] int id) {

            using (AllProgramEntities entities = new AllProgramEntities()) {
                var owner = entities.Tasks.Where(c => c.TaskID == id).Single<Tasks>();
                if (owner == null)
                    return "Not Found";
                entities.Tasks.Remove(owner);
                entities.SaveChanges();

            }


            return "Deleted Tasks: " + id;
        }
    }
 }