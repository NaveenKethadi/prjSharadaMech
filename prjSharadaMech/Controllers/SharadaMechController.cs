using prjSharadaMech.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjSharadaMech.Controllers
{
    public class SharadaMechController : Controller
    {
        // GET: SharadaMech
        string error = "";
        ClsDataAcceslayer obj = new ClsDataAcceslayer();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult GetDetailsBOM(int CompanyId, string SessionId, int LoginId, int vtype, string DocNo)
        {
            try
            {
                string opitemname = "";
                string opitemcode = "";
                int opitemid = 0; decimal oqty = 0.0M; int odefbaseunit = 0; int variantid1 = 0;
                int dreplinishment = 0; string narration = ""; int bomid = 0; int bomid1 = 0; long createdmodifieddate = 0; int variantid = 0;
                string qry1 = $@"select iDate Date,isnull(sNarration,'') Narration,isnull(VersionNo,'') VersionNo,
                            mp1.iMasterId outputitemid,mp1.sCode [Output Item Code],mp1.sName [Output Item],mpr.iDefaultReplenishment,mpu1.iDefaultBaseUnit [Op Default Base Unit],mpu2.iDefaultPurchaseUnit [ip Default Base Unit],
                            mp.iMasterId inputitemid,mp.sCode [Input Item Code],mp.sName InputItem,isnull(mp.sName,'') Description,minput3 [RqQty],ind.fQuantity Quantity,ind.mRate Rate
                            from tCore_Header_0 h join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                            join tCore_HeaderData7938_0 thv on thv.iHeaderId=d.iHeaderId
                            join tCore_Data7938_0 tdv on tdv.iBodyId=d.iBodyId
                            join tCore_Indta_0 ind on ind.iBodyId=d.iBodyId
                            --join tCore_Data_Tags_0 tdt on tdt.iBodyId=d.iBodyId
                            join mCore_Product mp on mp.iMasterId=iProduct
                            join mCore_Product mp1 on mp1.iMasterId=tdv.OutputItem
                            join muCore_Product_Replenishment mpr on mpr.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu1 on mpu1.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu2 on mpu2.iMasterId=mp.iMasterId
                             --join mCore_Units mu on mu.iMasterId=iUnit
                            join tCore_IndtaBodyScreenData_0 indb on indb.iBodyId=d.iBodyId 
                            where iVoucherType={vtype} and sVoucherNo='{DocNo}' order by mp1.iMasterId desc";
                DataSet ds1 = ClsDataAcceslayer.GetData1(qry1, CompanyId, ref error);
                List<BOMMasterModel> bommodellist = new List<Models.BOMMasterModel>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        BOMMasterModel bommodelsin = new Models.BOMMasterModel();
                        bommodelsin.Date = Convert.ToInt32(ds1.Tables[0].Rows[i]["Date"]);
                        bommodelsin.Narration = ds1.Tables[0].Rows[i]["Narration"].ToString();
                        bommodelsin.Output_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["outputitemid"]);
                        bommodelsin.Output_Item_Code = ds1.Tables[0].Rows[i]["Output Item Code"].ToString();
                        bommodelsin.Output_Item = ds1.Tables[0].Rows[i]["Output Item"].ToString();
                        bommodelsin.DefaultReplinishment = Convert.ToInt32(ds1.Tables[0].Rows[i]["iDefaultReplenishment"]);
                        bommodelsin.Output_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["Op Default Base Unit"]);
                        bommodelsin.Input_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["ip Default Base Unit"]);
                        bommodelsin.Input_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["inputitemid"]);
                        bommodelsin.Input_Item_Code = ds1.Tables[0].Rows[i]["Input Item Code"].ToString();
                        bommodelsin.Input_Item = ds1.Tables[0].Rows[i]["InputItem"].ToString();
                        bommodelsin.Description = ds1.Tables[0].Rows[i]["Description"].ToString();
                        bommodelsin.Req_Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["RqQty"]);
                        bommodelsin.Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Quantity"]);
                        bommodelsin.Rate = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Rate"]);
                        bommodellist.Add(bommodelsin);
                    }
                }
                var bommasterlst = bommodellist.FirstOrDefault();
                opitemname = bommasterlst.Output_Item;
                opitemcode = bommasterlst.Output_Item_Code;
                opitemid = bommasterlst.Output_Item_Id;
                dreplinishment = bommasterlst.DefaultReplinishment;
                oqty = bommasterlst.Quantity;
                narration = bommasterlst.Narration;
                odefbaseunit = bommasterlst.Output_DefaultBaseUnit;
                // string getbomid = $@"select bomh.iBomId from mMRP_BomHeader bomh join mMRP_BomVariantHeader
                //          bomvar on bomh.iBomId=bomvar.iBomId where bomh.sName='{opitemname}' and bomvar.sName='{opitemname}'";
                  string getbomid1 = $@" select bomh.iBomId from mMRP_BomHeader bomh join mMRP_BomVariantHeader
                          bomvar on bomh.iBomId=bomvar.iBomId and bomh.iStatus<>5 where bomh.sName='{opitemname}' --and bomvar.sName='{opitemname}'
                             and bomvar.iStatus<>5";
                DataSet dsgetbomid1 = ClsDataAcceslayer.GetData1(getbomid1, CompanyId, ref error);
                if (dsgetbomid1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsgetbomid1.Tables[0].Rows.Count; i++)
                    {
                        bomid1 = Convert.ToInt32(dsgetbomid1.Tables[0].Rows[i]["iBomId"]);
                        //variantid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iVaraintId"]);
                    }
                }
                if (bomid1 > 0)
                {
                    // string getbomid = $@"select bh.iBomId,prodord.iVaraintId,bh.sName hName,bvh.sName vhname from mMRP_BomVariantHeader bvh join mMRP_BomHeader bh on bvh.iBomId=bh.iBomId
                    //                      join tMrp_ProdOrder_0 prodord on prodord.iVaraintId=bvh.iVariantId
                    //                     where iVariantId=2598";
                    string getbomid = $@"select bh.iBomId,prodord.iVaraintId,bh.sName hName,bvh.sName vhname from 
                                     mMRP_BomVariantHeader bvh join mMRP_BomHeader bh on bvh.iBomId=bh.iBomId
                                     join tMrp_ProdOrder_0 prodord on prodord.iVaraintId=bvh.iVariantId
                                     where bh.sName='{opitemname}'";
                    DataSet dsgetbomid = ClsDataAcceslayer.GetData1(getbomid, CompanyId, ref error);
                    if (dsgetbomid.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsgetbomid.Tables[0].Rows.Count; i++)
                        {
                            bomid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iBomId"]);
                            variantid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iVaraintId"]);
                        }
                    }
                    if (bomid > 0)
                    {
                        //return to create version for exisiting bom
                        return Json(new { status = true, bomid1, variantid });
                        #region oldcodefor version
                        // long moddt = 0;
                        // string varname = "";int n01 = 0;int n02 = 0;
                        // int count = 0;
                        // string updateinbh = $@"update mMRP_BomHeader set iModifiedDate=dbo.fCore_DateTimeToInt(getdate())  where iBomId={bomid} and iStatus<>5";
                        // int bhupdate = obj.GetQueryExe(updateinbh, CompanyId, ref error);                  
                        // string getupdatefrmbh = $@"select imodifieddate from mMRP_BomHeader where iBomId={bomid} and iStatus<>5";
                        // //string umoddt = $@"select iModifiedDate from mMRP_BomVariantHeader where iBomId={bomid}";
                        // DataSet dsmoddt = ClsDataAcceslayer.GetData1(getupdatefrmbh, CompanyId, ref error);
                        // if (dsmoddt.Tables[0].Rows.Count > 0)
                        // {
                        //     for (int i = 0; i < dsmoddt.Tables[0].Rows.Count; i++)
                        //     {
                        //         moddt = Convert.ToInt64(dsmoddt.Tables[0].Rows[i]["iModifiedDate"]);
                        //     }
                        // }
                        // string updtmoddt = $@"update mMRP_BomVariantHeader set iModifiedDate={moddt} where iBomId={bomid} and iStatus<>5";
                        // int updt = obj.GetQueryExe(updtmoddt, CompanyId, ref error);
                        // string cntqry = $@"select count(*) count from mMRP_BomVariantHeader where iBomId={bomid} and iStatus<>5";
                        // DataSet dscnt = ClsDataAcceslayer.GetData1(cntqry, CompanyId, ref error);
                        // if (dscnt.Tables[0].Rows.Count > 0)
                        // {
                        //     for (int i = 0; i < dscnt.Tables[0].Rows.Count; i++)
                        //     {
                        //         count = Convert.ToInt32(dscnt.Tables[0].Rows[i]["count"]);
                        //     }
                        // }
                        // // if(count>0)
                        // // {
                        //// count = count + 1;
                        // varname = opitemname + " V" + count;
                        // // }

                        // string insertbomvarhead = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,iModifiedDate) Values
                        //                                                    ('{varname}',0,{bomid},1,1,1,{moddt},{moddt})";
                        // int n00 = obj.GetQueryExe(insertbomvarhead, CompanyId, ref error);
                        // string getvariantid = $@"select  top 1  iVariantId from mMRP_BomVariantHeader where iBomId={bomid} and iStatus<>5 order by iVariantId desc";
                        // DataSet dsvariantid = ClsDataAcceslayer.GetData1(getvariantid, CompanyId, ref error);
                        // if (dsvariantid.Tables[0].Rows.Count > 0)
                        // {
                        //     for (int i = 0; i < dsvariantid.Tables[0].Rows.Count; i++)
                        //     {
                        //         variantid = Convert.ToInt32(dsvariantid.Tables[0].Rows[i]["iVariantId"]);
                        //     }
                        // }
                        // string bombobyopqry1 = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                        //,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                        //,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                        //values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                        //  n01 = obj.GetQueryExe(bombobyopqry1, CompanyId, ref error);
                        // foreach (var item in bommodellist)
                        // {
                        //     string bombodyipqry1 = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                        //                     ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                        //                     ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                        //                      iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                        //      n02 = obj.GetQueryExe(bombodyipqry1, CompanyId, ref error);
                        // }
                        // string versionfield = "";
                        // if(n01>0 &&n02>0)
                        // {
                        //     string verqry = $@"select isnull(VersionNo,'') VersionNo from tCore_Header_0 h join 
                        //   tCore_HeaderData7938_0 thv on thv.iHeaderId=h.iHeaderId where iVoucherType=7938 and sVoucherNo='{DocNo}'";
                        //     DataSet dsver = ClsDataAcceslayer.GetData1(verqry,CompanyId,ref error);
                        //     if(dsver.Tables[0].Rows.Count>0)
                        //     {
                        //         for (int i = 0; i < dsver.Tables[0].Rows.Count; i++) {
                        //             versionfield = dsver.Tables[0].Rows[i]["VersionNo"].ToString(); }                           
                        //     }
                        //     if(versionfield=="")
                        //     {
                        //         versionfield = varname;
                        //     }
                        //     else { versionfield = versionfield+ "," + varname; }
                        //     string verupdatqry = $@" update tCore_HeaderData7938_0 set VersionNo='{versionfield}' where iHeaderId=
                        //                         (select h.iHeaderId from tCore_Header_0 h join  tCore_HeaderData7938_0
                        //                   thv on thv.iHeaderId=h.iHeaderId where iVoucherType=7938 and sVoucherNo='{DocNo}')";
                        //     int ver = obj.GetQueryExe(verupdatqry,CompanyId,ref error);
                        // }
                        #endregion
                    }
                    else
                    {
                        //return to update existing bom
                        return Json(new { status = false, bomid1, variantid });
                        #region insertcode
                        //     string bomheaderqry = $@"insert into mMRP_BomHeader (sName,sCode,sDescription,bDefault,iType,iFrom,iTo,iSize,iVersion,bAuthorized,iCreatedBy,iModifiedBy,
                        //                 iCreatedDate,iModifiedDate,iLocationId,bEditedFrom,iEditingLocation,iStatus,iTagId,iTagValue,sRemarks,iBOMRateOptions,iBOMPriceBookId,iBOMPriceBookFieldId,iAuthStatus)
                        //                 values( '{opitemname}','{opitemcode}','{opitemname}',1,0,dbo.DateToInt(GETDATE()),dbo.DateToInt(GETDATE()),1,0,0,1,1,dbo.fCore_DateTimeToInt(GETDATE()),
                        //                 dbo.fCore_DateTimeToInt(GETDATE()),0,0,0,1,0,0,'{narration}',0,0,0,0)";
                        //     int n1 = obj.GetQueryExe(bomheaderqry, CompanyId, ref error);
                        //     string getbomidqry = $@"select iBomId,iCreatedDate from mMRP_BomHeader where sName='{opitemname}'";
                        //     DataSet dsbomid = ClsDataAcceslayer.GetData1(getbomidqry, CompanyId, ref error);
                        //     if (dsbomid.Tables[0].Rows.Count > 0)
                        //     {
                        //         for (int i = 0; i < dsbomid.Tables[0].Rows.Count; i++)
                        //         {
                        //             bomid = Convert.ToInt32(dsbomid.Tables[0].Rows[i]["iBomId"]);
                        //             createdmodifieddate = Convert.ToInt64(dsbomid.Tables[0].Rows[i]["iCreatedDate"]);
                        //         }

                        //     }
                        //     string bomvariantheaderqry = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,
                        //                         iModifiedDate) Values('{opitemname}',0,{bomid},1,1,1,{createdmodifieddate},{createdmodifieddate})";
                        //     int n2 = obj.GetQueryExe(bomvariantheaderqry, CompanyId, ref error);
                        //     string getvaridqry = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid}";
                        //     DataSet dsvarid = ClsDataAcceslayer.GetData1(getvaridqry, CompanyId, ref error);
                        //     if (dsvarid.Tables[0].Rows.Count > 0)
                        //     {
                        //         for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                        //         {
                        //             variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                        //         }
                        //     }
                        //     string bombobyopqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                        //,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                        //,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                        //values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                        //     int n3 = obj.GetQueryExe(bombobyopqry, CompanyId, ref error);
                        //     foreach (var item in bommodellist)
                        //     {
                        //         string bombodyipqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                        //                     ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                        //                     ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                        //                      iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                        //         int n4 = obj.GetQueryExe(bombodyipqry, CompanyId, ref error);
                        //     }
                        #endregion
                    }
                }

                // return Json(new { status = true, Message = "Saved into bill of matarial(BOM)" });
                else
                {
                    //creating new bom
                    #region BOMcreation
                    string bomheaderqry = $@"insert into mMRP_BomHeader (sName,sCode,sDescription,bDefault,iType,iFrom,iTo,iSize,iVersion,bAuthorized,iCreatedBy,iModifiedBy,
                                     iCreatedDate,iModifiedDate,iLocationId,bEditedFrom,iEditingLocation,iStatus,iTagId,iTagValue,sRemarks,iBOMRateOptions,iBOMPriceBookId,iBOMPriceBookFieldId,iAuthStatus)
                                     values( '{opitemname}','{opitemcode}','{opitemname}',1,0,dbo.DateToInt(GETDATE()),dbo.DateToInt(GETDATE()),1,0,0,1,1,dbo.fCore_DateTimeToInt(GETDATE()),
                                     dbo.fCore_DateTimeToInt(GETDATE()),0,0,0,1,0,0,'{narration}',0,0,0,0)";
                    int n1 = obj.GetQueryExe(bomheaderqry, CompanyId, ref error);
                    string getbomidqry = $@"select iBomId,iCreatedDate from mMRP_BomHeader where sName='{opitemname}' and iStatus<>5";
                    DataSet dsbomid = ClsDataAcceslayer.GetData1(getbomidqry, CompanyId, ref error);
                    if (dsbomid.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsbomid.Tables[0].Rows.Count; i++)
                        {
                            bomid = Convert.ToInt32(dsbomid.Tables[0].Rows[i]["iBomId"]);
                            createdmodifieddate = Convert.ToInt64(dsbomid.Tables[0].Rows[i]["iCreatedDate"]);
                        }

                    }
                    string bomvariantheaderqry = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,
                                             iModifiedDate) Values('{opitemname}',0,{bomid},1,1,1,{createdmodifieddate},{createdmodifieddate})";
                    int n2 = obj.GetQueryExe(bomvariantheaderqry, CompanyId, ref error);
                    string getvaridqry = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid}";
                    DataSet dsvarid = ClsDataAcceslayer.GetData1(getvaridqry, CompanyId, ref error);
                    if (dsvarid.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                        {
                            variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                        }
                    }
                    string bombobyopqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                    ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                    ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                    values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                    int n3 = obj.GetQueryExe(bombobyopqry, CompanyId, ref error);
                    foreach (var item in bommodellist)
                    {
                        string bombodyipqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                                         ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                                         ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                                          iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                        int n4 = obj.GetQueryExe(bombodyipqry, CompanyId, ref error);
                    }
                    return Json(new { Message = "Saved into bill of matarial(BOM)" });
                    #endregion
                }
            }
            catch(Exception ex)
            {
                return Json(new {  Message = ex.Message });
            }
        }
        public ActionResult CreateVarint(int CompanyId, int BomId, int vtype, string DocNo)
        {
            try
            {
                string versionfield = "";
                string opitemname = "";
                string opitemcode = "";
                int opitemid = 0; decimal oqty = 0.0M; int odefbaseunit = 0;
                int dreplinishment = 0; string narration = ""; int bomid = 0; long createdmodifieddate = 0; int variantid = 0;
                string qry1 = $@"select iDate Date,isnull(sNarration,'') Narration,isnull(VersionNo,'') VersionNo,
                            mp1.iMasterId outputitemid,mp1.sCode [Output Item Code],mp1.sName [Output Item],mpr.iDefaultReplenishment,mpu1.iDefaultBaseUnit [Op Default Base Unit],mpu2.iDefaultPurchaseUnit [ip Default Base Unit],
                            mp.iMasterId inputitemid,mp.sCode [Input Item Code],mp.sName InputItem,isnull(mp.sName,'') Description,minput3 [RqQty],ind.fQuantity Quantity,ind.mRate Rate
                            from tCore_Header_0 h join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                            join tCore_HeaderData7938_0 thv on thv.iHeaderId=d.iHeaderId
                            join tCore_Data7938_0 tdv on tdv.iBodyId=d.iBodyId
                            join tCore_Indta_0 ind on ind.iBodyId=d.iBodyId
                            --join tCore_Data_Tags_0 tdt on tdt.iBodyId=d.iBodyId
                            join mCore_Product mp on mp.iMasterId=iProduct
                            join mCore_Product mp1 on mp1.iMasterId=tdv.OutputItem
                            join muCore_Product_Replenishment mpr on mpr.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu1 on mpu1.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu2 on mpu2.iMasterId=mp.iMasterId
                             --join mCore_Units mu on mu.iMasterId=iUnit
                            join tCore_IndtaBodyScreenData_0 indb on indb.iBodyId=d.iBodyId 
                            where iVoucherType={vtype} and sVoucherNo='{DocNo}' order by mp1.iMasterId desc";
                DataSet ds1 = ClsDataAcceslayer.GetData1(qry1, CompanyId, ref error);
                List<BOMMasterModel> bommodellist = new List<Models.BOMMasterModel>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        BOMMasterModel bommodelsin = new Models.BOMMasterModel();
                        bommodelsin.Date = Convert.ToInt32(ds1.Tables[0].Rows[i]["Date"]);
                        bommodelsin.Narration = ds1.Tables[0].Rows[i]["Narration"].ToString();
                        bommodelsin.Output_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["outputitemid"]);
                        bommodelsin.Output_Item_Code = ds1.Tables[0].Rows[i]["Output Item Code"].ToString();
                        bommodelsin.Output_Item = ds1.Tables[0].Rows[i]["Output Item"].ToString();
                        bommodelsin.DefaultReplinishment = Convert.ToInt32(ds1.Tables[0].Rows[i]["iDefaultReplenishment"]);
                        bommodelsin.Output_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["Op Default Base Unit"]);
                        bommodelsin.Input_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["ip Default Base Unit"]);
                        bommodelsin.Input_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["inputitemid"]);
                        bommodelsin.Input_Item_Code = ds1.Tables[0].Rows[i]["Input Item Code"].ToString();
                        bommodelsin.Input_Item = ds1.Tables[0].Rows[i]["InputItem"].ToString();
                        bommodelsin.Description = ds1.Tables[0].Rows[i]["Description"].ToString();
                        bommodelsin.Req_Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["RqQty"]);
                        bommodelsin.Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Quantity"]);
                        bommodelsin.Rate = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Rate"]);
                        bommodelsin.VersionNo = ds1.Tables[0].Rows[i]["VersionNo"].ToString();
                        bommodellist.Add(bommodelsin);
                    }
                }
                var bommasterlst = bommodellist.FirstOrDefault();
                opitemname = bommasterlst.Output_Item;
                opitemcode = bommasterlst.Output_Item_Code;
                opitemid = bommasterlst.Output_Item_Id;
                dreplinishment = bommasterlst.DefaultReplinishment;
                oqty = bommasterlst.Quantity;
                narration = bommasterlst.Narration;
                odefbaseunit = bommasterlst.Output_DefaultBaseUnit;
                versionfield = bommasterlst.VersionNo;
                long moddt = 0;
                string varname = ""; int n01 = 0; int n02 = 0;
                int count = 0;
                int versionno = 0;
               
                #region createVariant
                //"update mMRP_BomHeader set sRemarks='{narration}',iType=0,iModifiedDate=dbo.fCore_DateTimeToInt(getdate()) 
                //  where ibomid = { BomId } and iStatus<> 5
                //string updateinbh = $@"update mMRP_BomHeader set sRemarks='{narration}',iModifiedDate=dbo.fCore_DateTimeToInt(getdate())  where iBomId={BomId} and iStatus<>5";
                //int bhupdate = obj.GetQueryExe(updateinbh, CompanyId, ref error);
                //string getupdatefrmbh = $@"select imodifieddate from mMRP_BomHeader where iBomId={BomId} and iStatus<>5";
                ////string umoddt = $@"select iModifiedDate from mMRP_BomVariantHeader where iBomId={bomid}";
                //DataSet dsmoddt = ClsDataAcceslayer.GetData1(getupdatefrmbh, CompanyId, ref error);
                //if (dsmoddt.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsmoddt.Tables[0].Rows.Count; i++)
                //    {
                //        moddt = Convert.ToInt64(dsmoddt.Tables[0].Rows[i]["iModifiedDate"]);
                //    }
                //}
                //string updtmoddt = $@"update mMRP_BomVariantHeader set iModifiedDate={moddt} where iBomId={BomId} and iStatus<>5";
                //int updt = obj.GetQueryExe(updtmoddt, CompanyId, ref error);
                //string cntqry = $@"select count(*) count from mMRP_BomVariantHeader where iBomId={BomId} and iStatus<>5";
                //DataSet dscnt = ClsDataAcceslayer.GetData1(cntqry, CompanyId, ref error);
                //if (dscnt.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dscnt.Tables[0].Rows.Count; i++)
                //    {
                //        count = Convert.ToInt32(dscnt.Tables[0].Rows[i]["count"]);
                //    }
                //}
                //// if(count>0)
                //// {
                //// count = count + 1;
                //varname = opitemname + " V" + count;
                //// }

                //string insertbomvarhead = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,iModifiedDate) Values
                //                                                    ('{varname}',0,{BomId},1,1,1,{moddt},{moddt})";
                //int n00 = obj.GetQueryExe(insertbomvarhead, CompanyId, ref error);
                //string getvariantid = $@"select  top 1  iVariantId from mMRP_BomVariantHeader where iBomId={BomId} and iStatus<>5 order by iVariantId desc";
                //DataSet dsvariantid = ClsDataAcceslayer.GetData1(getvariantid, CompanyId, ref error);
                //if (dsvariantid.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsvariantid.Tables[0].Rows.Count; i++)
                //    {
                //        variantid = Convert.ToInt32(dsvariantid.Tables[0].Rows[i]["iVariantId"]);
                //    }
                //}
                //string bombobyopqry1 = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                //,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                //,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                //values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                //n01 = obj.GetQueryExe(bombobyopqry1, CompanyId, ref error);
                //foreach (var item in bommodellist)
                //{
                //    string bombodyipqry1 = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                //                     ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                //                     ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                //                      iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                //    n02 = obj.GetQueryExe(bombodyipqry1, CompanyId, ref error);
                //}
                //string versionfield = "";
                //if (n01 > 0 && n02 > 0)
                //{
                //    string verqry = $@"select isnull(VersionNo,'') VersionNo from tCore_Header_0 h join 
                //   tCore_HeaderData7938_0 thv on thv.iHeaderId=h.iHeaderId where iVoucherType=7938 and sVoucherNo='{DocNo}'";
                //    DataSet dsver = ClsDataAcceslayer.GetData1(verqry, CompanyId, ref error);
                //    if (dsver.Tables[0].Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dsver.Tables[0].Rows.Count; i++)
                //        {
                //            versionfield = dsver.Tables[0].Rows[i]["VersionNo"].ToString();
                //        }
                //    }
                //    if (versionfield == "")
                //    {
                //        versionfield = varname;
                //    }
                //    else { versionfield = versionfield + "," + varname; }
                //    string verupdatqry = $@" update tCore_HeaderData7938_0 set VersionNo='{versionfield}' where iHeaderId=
                //                         (select h.iHeaderId from tCore_Header_0 h join  tCore_HeaderData7938_0
                //                   thv on thv.iHeaderId=h.iHeaderId where iVoucherType=7938 and sVoucherNo='{DocNo}')";
                //    int ver = obj.GetQueryExe(verupdatqry, CompanyId, ref error);
                //}
                #endregion
                string getverqry = $@"select top 1 iVersion from mMRP_BomHeader where sName='{opitemname}' and iStatus<>5 order by iVersion desc";
                DataSet dsver = ClsDataAcceslayer.GetData1(getverqry,CompanyId,ref error);
                if(dsver!=null && dsver.Tables[0].Rows.Count > 0)
                {
                    versionno = Convert.ToInt32(dsver.Tables[0].Rows[0]["iVersion"]);
                }
                string bomheaderqry = $@"insert into mMRP_BomHeader (sName,sCode,sDescription,bDefault,iType,iFrom,iTo,iSize,iVersion,bAuthorized,iCreatedBy,iModifiedBy,
                                     iCreatedDate,iModifiedDate,iLocationId,bEditedFrom,iEditingLocation,iStatus,iTagId,iTagValue,sRemarks,iBOMRateOptions,iBOMPriceBookId,iBOMPriceBookFieldId,iAuthStatus)
                                     values( '{opitemname}','{opitemcode}','{opitemname}',1,0,dbo.DateToInt(GETDATE()),dbo.DateToInt(GETDATE()),1,{versionno}+1,0,1,1,dbo.fCore_DateTimeToInt(GETDATE()),
                                     dbo.fCore_DateTimeToInt(GETDATE()),0,0,0,1,0,0,'{narration}',0,0,0,0)";
                int n1 = obj.GetQueryExe(bomheaderqry, CompanyId, ref error);
                string getbomidqry = $@"select iBomId,iCreatedDate from mMRP_BomHeader where sName='{opitemname}'";
                DataSet dsbomid = ClsDataAcceslayer.GetData1(getbomidqry, CompanyId, ref error);
                if (dsbomid.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsbomid.Tables[0].Rows.Count; i++)
                    {
                        bomid = Convert.ToInt32(dsbomid.Tables[0].Rows[i]["iBomId"]);
                        createdmodifieddate = Convert.ToInt64(dsbomid.Tables[0].Rows[i]["iCreatedDate"]);
                    }

                }
                string bomvariantheaderqry = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,
                                             iModifiedDate) Values('{opitemname}',0,{bomid},1,1,1,{createdmodifieddate},{createdmodifieddate})";
                int n2 = obj.GetQueryExe(bomvariantheaderqry, CompanyId, ref error);
                string getvaridqry = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid}";
                DataSet dsvarid = ClsDataAcceslayer.GetData1(getvaridqry, CompanyId, ref error);
                if (dsvarid.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                    {
                        variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                    }
                }
                string bombobyopqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                    ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                    ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                    values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                int n3 = obj.GetQueryExe(bombobyopqry, CompanyId, ref error);
                int n4 = 0;
                foreach (var item in bommodellist)
                {
                    string bombodyipqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                                         ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                                         ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                                          iInvTagValue) values({variantid},{item.Input_Item_Id},{item.Req_Quantity},{item.Req_Quantity},{item.Input_DefaultBaseUnit},0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{item.Rate},0,0,0,0,0)";
                   n4 = obj.GetQueryExe(bombodyipqry, CompanyId, ref error);
                }
                if (n3 > 0 && n4 > 0)
                {
                    if (versionfield == "")
                    {
                        versionfield = opitemname +" V"+ (versionno+1).ToString();
                    }
                    else { versionfield = versionfield + "," + opitemname + " V" + (versionno + 1).ToString(); }
                    string verupdatqry = $@" update tCore_HeaderData7938_0 set VersionNo='{versionfield}' where iHeaderId=
                                         (select h.iHeaderId from tCore_Header_0 h join  tCore_HeaderData7938_0
                                   thv on thv.iHeaderId=h.iHeaderId where iVoucherType=7938 and sVoucherNo='{DocNo}')";
                    int ver = obj.GetQueryExe(verupdatqry, CompanyId, ref error);
                }
                return Json(new { status = true, Message = "Bom's variant is created" });
            }
            catch(Exception ex)
            {
                return Json(new { status = false, Message = ex.Message });
            }
        }
        //CompanyId: logDetails.CompanyId, BomId: response.bomid, VariantId: response.variantid, vtype: logDetails.iVoucherType, DocNo: docNo
        public ActionResult UpdateVariant(int CompanyId, int BomId, int vtype, string DocNo)
        {
            try
            {
                string opitemname = "";
                string opitemcode = "";
                int opitemid = 0; decimal oqty = 0.0M; int odefbaseunit = 0;
                int dreplinishment = 0; string narration = ""; int bomid = 0; long createdmodifieddate = 0; int variantid = 0;
                #region oldcodee
                // int variantid = 0;
                //string getvaridqry = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={BomId}";
                //DataSet dsvarid = ClsDataAcceslayer.GetData1(getvaridqry, CompanyId, ref error);
                //if (dsvarid.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                //    {
                //        variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                //    }
                //}
                //     string bombobyopqry = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                //        ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                //        ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,iInvTagValue )
                //        values({variantid},{opitemid},{oqty},{oqty},{odefbaseunit},0,0,0,0,'{opitemname}',null,-1,0,0,0,0,0,0,0,1,0,0,100,0,0,0,0)";
                //     int n3 = obj.GetQueryExe(bombobyopqry, CompanyId, ref error);
                #endregion
                string qry1 = $@"select iDate Date,isnull(sNarration,'') Narration,isnull(VersionNo,'') VersionNo,
                            mp1.iMasterId outputitemid,mp1.sCode [Output Item Code],mp1.sName [Output Item],mpr.iDefaultReplenishment,mpu1.iDefaultBaseUnit [Op Default Base Unit],mpu2.iDefaultPurchaseUnit [ip Default Base Unit],
                            mp.iMasterId inputitemid,mp.sCode [Input Item Code],mp.sName InputItem,isnull(mp.sName,'') Description,minput3 [RqQty],ind.fQuantity Quantity,ind.mRate Rate
                            from tCore_Header_0 h join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                            join tCore_HeaderData7938_0 thv on thv.iHeaderId=d.iHeaderId
                            join tCore_Data7938_0 tdv on tdv.iBodyId=d.iBodyId
                            join tCore_Indta_0 ind on ind.iBodyId=d.iBodyId
                            --join tCore_Data_Tags_0 tdt on tdt.iBodyId=d.iBodyId
                            join mCore_Product mp on mp.iMasterId=iProduct
                            join mCore_Product mp1 on mp1.iMasterId=tdv.OutputItem
                            join muCore_Product_Replenishment mpr on mpr.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu1 on mpu1.iMasterId=mp1.iMasterId
                            join muCore_Product_Units mpu2 on mpu2.iMasterId=mp.iMasterId
                            --join mCore_Units mu on mu.iMasterId=iUnit
                            join tCore_IndtaBodyScreenData_0 indb on indb.iBodyId=d.iBodyId 
                            where iVoucherType={vtype} and sVoucherNo='{DocNo}' order by mp1.iMasterId desc";
                DataSet ds1 = ClsDataAcceslayer.GetData1(qry1, CompanyId, ref error);
                List<BOMMasterModel> bommodellist = new List<Models.BOMMasterModel>();
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                    {
                        BOMMasterModel bommodelsin = new Models.BOMMasterModel();
                        bommodelsin.Date = Convert.ToInt32(ds1.Tables[0].Rows[i]["Date"]);
                        bommodelsin.Narration = ds1.Tables[0].Rows[i]["Narration"].ToString();
                        bommodelsin.Output_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["outputitemid"]);
                        bommodelsin.Output_Item_Code = ds1.Tables[0].Rows[i]["Output Item Code"].ToString();
                        bommodelsin.Output_Item = ds1.Tables[0].Rows[i]["Output Item"].ToString();
                        bommodelsin.DefaultReplinishment = Convert.ToInt32(ds1.Tables[0].Rows[i]["iDefaultReplenishment"]);
                        bommodelsin.Output_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["Op Default Base Unit"]);
                        bommodelsin.Input_DefaultBaseUnit = Convert.ToInt32(ds1.Tables[0].Rows[i]["ip Default Base Unit"]);
                        bommodelsin.Input_Item_Id = Convert.ToInt32(ds1.Tables[0].Rows[i]["inputitemid"]);
                        bommodelsin.Input_Item_Code = ds1.Tables[0].Rows[i]["Input Item Code"].ToString();
                        bommodelsin.Input_Item = ds1.Tables[0].Rows[i]["InputItem"].ToString();
                        bommodelsin.Description = ds1.Tables[0].Rows[i]["Description"].ToString();
                        bommodelsin.Req_Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["RqQty"]);
                        bommodelsin.Quantity = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Quantity"]);
                        bommodelsin.Rate = Convert.ToDecimal(ds1.Tables[0].Rows[i]["Rate"]);
                        bommodellist.Add(bommodelsin);
                    }
                }
                var bommasterlst = bommodellist.FirstOrDefault();
                opitemname = bommasterlst.Output_Item;
                opitemcode = bommasterlst.Output_Item_Code;
                opitemid = bommasterlst.Output_Item_Id;
                dreplinishment = bommasterlst.DefaultReplinishment;
                oqty = bommasterlst.Quantity;
                narration = bommasterlst.Narration;
                odefbaseunit = bommasterlst.Output_DefaultBaseUnit;
              string getvariantid1 = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={BomId} and iStatus<>5";
                DataSet dsvarid1 = ClsDataAcceslayer.GetData1(getvariantid1, CompanyId, ref error);
                if (dsvarid1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsvarid1.Tables[0].Rows.Count; i++)
                    {
                        variantid = Convert.ToInt32(dsvarid1.Tables[0].Rows[i]["iVariantId"]);
                    }
                }
                string updateOqry = $@"update mMRP_BomHeader set sRemarks='{narration}',iType=0,iModifiedDate=dbo.fCore_DateTimeToInt(getdate()) 
                                      where ibomid={BomId} and iStatus<>5
update mMRP_BomVariantHeader set iModifiedDate=dbo.fCore_DateTimeToInt(getdate()) where iVariantId={variantid} and ibomid={BomId}
update mMRP_BOMBody set iProductId={opitemid},fQty={oqty},fQty2={oqty},iUnit={odefbaseunit},sDescription='{opitemname}' 
                                                                           where iVariantId={variantid} and binput=0";
                int n0 = obj.GetQueryExe(updateOqry, CompanyId, ref error);
                List<int> varinputids=new List<int>();
                string varipidqry = $@"select iBomBodyId from mMRP_BOMBody where iVariantId={variantid} and bInput=1";
                DataSet dsvaripid = ClsDataAcceslayer.GetData1(varipidqry,CompanyId,ref error);
                if(dsvaripid.Tables[0].Rows.Count>0)
                {
                    for(int i=0;i< dsvaripid.Tables[0].Rows.Count;i++)
                    {
                        varinputids.Add(Convert.ToInt32(dsvaripid.Tables[0].Rows[i]["iBomBodyId"]));
                    }
                }
    // $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
    //  ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
    //  ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
    // InvTagValue) values({ variantid},{ item.Input_Item_Id},{ item.Req_Quantity},{ item.Req_Quantity},{ item.Input_DefaultBaseUnit},
    //0.00,0,0,0,'{item.Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{ item.Rate},0,0,0,0,0)";
                for (int j=0;j< bommodellist.Count;j++)
                {
                    if (j < varinputids.Count) { 
                    string updateIqry = $@"update mMRP_BOMBody set iProductId={bommodellist[j].Input_Item_Id},fQty={bommodellist[j].Req_Quantity},fQty2={bommodellist[j].Req_Quantity},iUnit={bommodellist[j].Input_DefaultBaseUnit},
                                          fRate={bommodellist[j].Rate},sDescription='{bommodellist[j].Input_Item}' where iVariantId={variantid} and binput=1 and iBomBodyId={varinputids[j]}";
                    int n1= obj.GetQueryExe(updateIqry, CompanyId, ref error);
                    }
                    else
                    {
                        //string insertbomvarhead = $@"insert into mMRP_BomVariantHeader (sName,bDefault,iBomId,iStatus,iAuthStatus,iUserId,iCreatedDate,iModifiedDate) Values
                        //                                            ('{bommodellist[j].Input_Item}',0,{BomId},1,1,1,{moddt},{moddt})";
                        // int n00 = obj.GetQueryExe(insertbomvarhead, CompanyId, ref error);
                        string bombodyipqry1 = $@"insert into mMRP_BOMBody (iVariantId,iProductId,fQty,fQty2,iUnit,dScrapPerc,iProcureType,iFrom,iTo
                                     ,sDescription,sFormula,iAlternateOf,bInput,dTrailScrapPerc,dTolerancePerc,bFixedQty,bRoundOff,iPriority
                                     ,iScrapType,bMainOutPut,bDonotIncludeInCosting,fRate,dCostPercentage, bPercentageEdited,fTolerence,iRowIndex,
                                      iInvTagValue) values({variantid},{bommodellist[j].Input_Item_Id},{bommodellist[j].Req_Quantity},{bommodellist[j].Req_Quantity},{bommodellist[j].Input_DefaultBaseUnit},0.00,0,0,0,'{bommodellist[j].Input_Item}',null,-1,1,0,0,0,0,0,0,0,0,{bommodellist[j].Rate},0,0,0,0,0)";
                       int n2 = obj.GetQueryExe(bombodyipqry1, CompanyId, ref error);
                    }
                }
                return Json(new { status = true, Message = "BOM updated sucessfully" });
            }
            catch(Exception ex)
            {
                return Json(new { status = false, Message = ex.Message });
            }
        }
        //GetDetailsBOMForDelete
        public ActionResult GetDetailsBOMForDelete(int CompanyId, string SessionId, int LoginId, int vtype, string DocNo)
        {
            string opitem = "";
          //  string docno = "BOM : " + DocNo;
            string prodord = "";
           string sname = "";
           int bomid = 0;
            string Message = "";int variantid = 0;
            try
            {
                string getopitemqry = $@"select OutputItem,mp1.sName from tcore_header_0 h join tcore_data_0 d on h.iHeaderId=d.iHeaderId
	                                 join tCore_Data7938_0 tdv on tdv.iBodyId=d.iBodyId
	                                 join mCore_Product mp1 on mp1.iMasterId=tdv.OutputItem
	                                 where iVoucherType=7938 and sVoucherNo='{DocNo}' and outputitem<>0";
                DataSet dsopitem = ClsDataAcceslayer.GetData1(getopitemqry, CompanyId, ref error);
                if (dsopitem.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsopitem.Tables[0].Rows.Count; i++)
                    {
                        opitem = dsopitem.Tables[0].Rows[i]["OutputItem"].ToString();
                        sname = dsopitem.Tables[0].Rows[i]["sName"].ToString();
                    }
                }
                string getbomid = $@"select bh.iBomId,prodord.iVaraintId,bh.sName hName,bvh.sName vhname from 
                                     mMRP_BomVariantHeader bvh join mMRP_BomHeader bh on bvh.iBomId=bh.iBomId
                                     join tMrp_ProdOrder_0 prodord on prodord.iVaraintId=bvh.iVariantId
                                     where bh.sName='{sname}'";
                DataSet dsgetbomid = ClsDataAcceslayer.GetData1(getbomid, CompanyId, ref error);
                if (dsgetbomid.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsgetbomid.Tables[0].Rows.Count; i++)
                    {
                        bomid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iBomId"]);
                        variantid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iVaraintId"]);
                    }
                }
                if (bomid > 0 && variantid > 0)
                {
                    return Json(new { status = false, Message = "Transaction made for this BOM,not allowed to delete" });
                }
                else
                {
                    int variantid1 = 0;
                    int bomid1 = 0;
                    string getbomid1 = $@"select bomh.iBomId from mMRP_BomHeader bomh join mMRP_BomVariantHeader
                             bomvar on bomh.iBomId=bomvar.iBomId where bomh.sName='{sname}' and bomvar.sName='{sname}'";
                    DataSet dsgetbomid1 = ClsDataAcceslayer.GetData1(getbomid1, CompanyId, ref error);
                    if (dsgetbomid1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsgetbomid1.Tables[0].Rows.Count; i++)
                        {
                            bomid1 = Convert.ToInt32(dsgetbomid1.Tables[0].Rows[i]["iBomId"]);
                        }
                    }
                    string getvariantid1 = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid1} and iStatus<>5";
                    DataSet dsvarid1 = ClsDataAcceslayer.GetData1(getvariantid1, CompanyId, ref error);
                    if (dsvarid1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsvarid1.Tables[0].Rows.Count; i++)
                        {
                            variantid1 = Convert.ToInt32(dsvarid1.Tables[0].Rows[i]["iVariantId"]);
                        }
                    }
                    string deleteqry = $@"delete from mMRP_BOMBody where iVariantId in ({variantid1})
                                         delete from mMRP_BomVariantHeader where ibomid={bomid1}
                                         delete from mMRP_BomHeader where ibomid={bomid1}";
                    int n = obj.GetQueryExe(deleteqry, CompanyId, ref error);

                }
                //string getbomid1 = $@" select iBomId from mMRP_BomHeader where sName='{sname}' and iStatus<>5";
                //DataSet dsgetbomid1 = ClsDataAcceslayer.GetData1(getbomid1, CompanyId, ref error);
                //if (dsgetbomid1.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsgetbomid1.Tables[0].Rows.Count; i++)
                //    {
                //        bomid1 = Convert.ToInt32(dsgetbomid1.Tables[0].Rows[i]["iBomId"]);
                //        //variantid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iVaraintId"]);
                //    }
                //}

                //string prodordqry = $@"select svoucherno from tCore_Header_0 h join tCore_Data_0 d on h.iHeaderId=d.iHeaderId
                //                     join tCore_Indta_0 ind on ind.iBodyId=d.iBodyId
                //                     join vtCore_LinksData_0 vtl on vtl.iBodyId=d.iBodyId
                //                     join tCore_Data7939_0 tdv on tdv.iBodyId=d.iBodyId
                //                     where iVoucherType=7939 and OutputItem<>0 and OutputItem={opitem} and sLinkVoucherNo='{docno}'";
                //DataSet dsprodordqry = ClsDataAcceslayer.GetData1(prodordqry, CompanyId, ref error);
                //if (dsprodordqry.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsprodordqry.Tables[0].Rows.Count; i++)
                //    {
                //        prodord = dsprodordqry.Tables[0].Rows[i]["svoucherno"].ToString();
                //    }
                //}
                //if(prodord!="")
                //{
                //    return Json(new { status=false,Message="Transaction made for this BOM,not allowed to delete"});
                //}
                //else
                //{
                //    int variantid = 0;
                //    string getbomid = $@"select bomh.iBomId from mMRP_BomHeader bomh join mMRP_BomVariantHeader
                //             bomvar on bomh.iBomId=bomvar.iBomId where bomh.sName='{sname}' and bomvar.sName='{sname}'";
                //    DataSet dsgetbomid = ClsDataAcceslayer.GetData1(getbomid, CompanyId, ref error);
                //    if (dsgetbomid.Tables[0].Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dsgetbomid.Tables[0].Rows.Count; i++)
                //        {
                //            bomid = Convert.ToInt32(dsgetbomid.Tables[0].Rows[i]["iBomId"]);
                //        }
                //    }
                //    string getvariantid = $@"select iVariantId from mMRP_BomVariantHeader where iBomId={bomid} and iStatus<>5";
                //    DataSet dsvarid = ClsDataAcceslayer.GetData1(getvariantid,CompanyId,ref error);
                //    if (dsvarid.Tables[0].Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dsvarid.Tables[0].Rows.Count; i++)
                //        {
                //            variantid = Convert.ToInt32(dsvarid.Tables[0].Rows[i]["iVariantId"]);
                //        }
                //    }
                //    string deleteqry = $@"delete from mMRP_BOMBody where iVariantId in ({variantid})
                //                         delete from mMRP_BomVariantHeader where ibomid={bomid}
                //                         delete from mMRP_BomHeader where ibomid={bomid}";
                //    int n= obj.GetQueryExe(deleteqry, CompanyId, ref error);
                //    if(n>0)
                //    {
                //        Message="Bom Deleted";
                //    }
                return Json(new { status = true, Message = "Deleted successfully" });
            }
            catch(Exception ex)
            {
                return Json(new { status = true, Message =ex.Message });
            }
        }
    }
           
    }
