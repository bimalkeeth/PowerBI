

export  class ValidationUtil {

  static IsEmail(search:string):boolean
  {
    let  searchFind:boolean;
    let regexp = new RegExp("^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z-])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$");
    searchFind = regexp.test(search);
    return searchFind
  }
   static IsNotEmpty(text:string):boolean{
    let searchFind:boolean=true;
    if(text==''||!text){
      searchFind=false;
    }
    return searchFind
  }
}
