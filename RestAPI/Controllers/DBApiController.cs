﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI.Controllers
{
    public class DBApiController : ApiController
    {
        // GET api/DBApi
        public IEnumerable<User> GetAllUsers()
        {
            using (RestAPIDBEntities db = new RestAPIDBEntities())
            {
                return db.Users.ToList();
            }
        }

        // GET api/DBApi/5
        public HttpResponseMessage Get(string id)
        {
            using (RestAPIDBEntities db = new RestAPIDBEntities())
            {
                var entity = db.Users.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with mentioned id not found");
                }
                else
                {
                    //entity.Password = DecodePasswordFrom64(entity.Password);
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
            }
        }

        // POST api/DBApi
        [HttpPost]
        public HttpResponseMessage Post([FromBody] User user)
        {
            using (RestAPIDBEntities db = new RestAPIDBEntities())
            {
                //return db.Users.FirstOrDefault(e => e.Id == id);

                var entity = db.Users.FirstOrDefault(e => e.EmailAddress == user.EmailAddress);
                if (entity == null)
                {
                    try
                    {
                        user.Id = System.Guid.NewGuid().ToString();
                        //user.Password = EncodePasswordToBase64(user.Password);
                        db.Users.Add(user);
                        db.SaveChanges();
                        var message = Request.CreateResponse(HttpStatusCode.Created, user);
                        message.Headers.Location = new Uri(Request.RequestUri + user.Id.ToString());
                        return message;
                    }
                    catch (Exception ex)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Found, "Email ID Already Exist");
                }
            }

            
        }

        // PUT api/DBApi/5
        [HttpPut]
        public HttpResponseMessage Put([FromBody]User user)
        {
            using (RestAPIDBEntities db = new RestAPIDBEntities())
            {
                try
                {
                        var usercheck = db.Users.SqlQuery("Select * from users where emailaddress='" + user.EmailAddress + "' and id!='" + user.Id + "'").FirstOrDefault<User>();
                    
                        if (usercheck == null)
                        {
                            var entity = db.Users.FirstOrDefault(e => e.Id == user.Id);
                            if (entity == null)
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with mentioned id not found to update");
                            }
                            else
                            {
                                entity.FirstName = user.FirstName;
                                entity.LastName = user.LastName;
                                entity.EmailAddress = user.EmailAddress;
                                //entity.Password = EncodePasswordToBase64(user.Password);
                                entity.Password = user.Password;
                                entity.ActiveStatus = user.ActiveStatus;
                                entity.LockStatus = user.LockStatus;
                                db.SaveChanges();
                                return Request.CreateResponse(HttpStatusCode.OK, entity);
                            }
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.Found, "Email ID already registered with another user");
                        }
                
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }
        }

        // DELETE api/DBApi/5
        [HttpDelete]
        public HttpResponseMessage Delete(string id)
        {
            using (RestAPIDBEntities db = new RestAPIDBEntities())
            {
                var entity = db.Users.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with id=" + id.ToString() + "not found to delete");
                }
                else
                {
                    db.Users.Remove(entity);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
        }
        
        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        } //this function Convert to Decord your Password

        public string DecodePasswordFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

    }
}
