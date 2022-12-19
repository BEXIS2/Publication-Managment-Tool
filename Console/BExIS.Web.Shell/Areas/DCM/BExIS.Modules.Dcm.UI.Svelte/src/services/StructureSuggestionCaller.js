// Implementations for all the calls for the pokemon endpoints.
//import Api from "./Api";
import {Api} from "@bexis2/bexis2-core-ui/src/lib/index";

/****************/ 
/* Create*/
/****************/ 
export const load = async (id, file, version) => {
 try {
   const response = await Api.get('/dcm/StructureSuggestion/load?id='+ id +'&&file='+file+'&&version='+version );
   return response.json();
 } catch (error) {
   console.error(error);
 }
};

export const getDelimeters = async (id, file, version) => {
  try {
    const response = await Api.get('/dcm/StructureSuggestion/GetDelimters' );
    return response.json();
  } catch (error) {
    console.error(error);
  }
};
 
export const generate = async (data) => {
  try {
    const response = await Api.post('/dcm/StructureSuggestion/generate', data);
    return response.json();
  } catch (error) {
    console.error(error);
  }
};

export const store = async (data) => {
  try {
    const response = await Api.post('/dcm/StructureSuggestion/store',data);
    return response.json();
  } catch (error) {
    console.error(error);
  }
};

export const save = async (data) => {
  try {
    const response = await Api.post('/dcm/StructureSuggestion/save',data);
    return response.json();
  } catch (error) {
    console.error(error);
  }
};

export const getDataTypes = async () => {
  try {
    const response = await Api.get('/dcm/StructureSuggestion/getDataTypes');
    return response.json();
  } catch (error) {
    console.error(error);
  }
};

export const getUnits = async () => {
  try {
    const response = await Api.get('/dcm/StructureSuggestion/getUnits');
    return response.json();
  } catch (error) {
    console.error(error);
  }
};