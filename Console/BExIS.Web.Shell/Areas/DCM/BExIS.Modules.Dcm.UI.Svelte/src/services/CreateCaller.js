// Implementations for all the calls for the pokemon endpoints.
//import Api from "./Api";
import {Api} from "@bexis2/bexis2-core-ui/src/lib/index";

/****************/ 
/* Create*/
/****************/ 
export const getCreate = async (id) => {
 try {
   const response = await Api.get('/dcm/create/get?id='+id);
   return response.json();
 } catch (error) {
   console.error(error);
 }
};

export const create = async (data) => {
  try {
    const response = await Api.post('/dcm/create/create', data);
    return response.json();;
  } catch (error) {
    console.error(error);
  }
 };
