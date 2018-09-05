﻿//------------------------------------------------------------------------------
// <auto-generated>
//    這個程式碼是由範本產生。
//
//    對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//    如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jasper.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class RENTEntities : DbContext
    {
        public RENTEntities()
            : base("name=RENTEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ACCBUDGET> ACCBUDGET { get; set; }
        public DbSet<ACCTP> ACCTP { get; set; }
        public DbSet<BROKER> BROKER { get; set; }
        public DbSet<BROKMAN> BROKMAN { get; set; }
        public DbSet<CARLOCT> CARLOCT { get; set; }
        public DbSet<CLASBOOK> CLASBOOK { get; set; }
        public DbSet<CLUBFEE> CLUBFEE { get; set; }
        public DbSet<CNEQUIP> CNEQUIP { get; set; }
        public DbSet<COMPLN> COMPLN { get; set; }
        public DbSet<COMPLNDO> COMPLNDO { get; set; }
        public DbSet<CONTRAA> CONTRAA { get; set; }
        public DbSet<CONTRAC> CONTRAC { get; set; }
        public DbSet<CONTRAF> CONTRAF { get; set; }
        public DbSet<CONTRAH> CONTRAH { get; set; }
        public DbSet<CONTRAP> CONTRAP { get; set; }
        public DbSet<COUNTRY> COUNTRY { get; set; }
        public DbSet<CTINF> CTINF { get; set; }
        public DbSet<DEALER> DEALER { get; set; }
        public DbSet<DEALMAN> DEALMAN { get; set; }
        public DbSet<DESKBOOK> DESKBOOK { get; set; }
        public DbSet<DESKCASH> DESKCASH { get; set; }
        public DbSet<DISCNTRA> DISCNTRA { get; set; }
        public DbSet<dtproperties> dtproperties { get; set; }
        public DbSet<EQUIP> EQUIP { get; set; }
        public DbSet<FIXM> FIXM { get; set; }
        public DbSet<HOMEAST> HOMEAST { get; set; }
        public DbSet<MAKR> MAKR { get; set; }
        public DbSet<MAKRMAN> MAKRMAN { get; set; }
        public DbSet<MBRM> MBRM { get; set; }
        public DbSet<NEWSTP> NEWSTP { get; set; }
        public DbSet<OTHERIN> OTHERIN { get; set; }
        public DbSet<PARKFEE> PARKFEE { get; set; }
        public DbSet<PAYFEE> PAYFEE { get; set; }
        public DbSet<PBRENTAL> PBRENTAL { get; set; }
        public DbSet<POSM> POSM { get; set; }
        public DbSet<PUBLISHER> PUBLISHER { get; set; }
        public DbSet<PUBLISHF> PUBLISHF { get; set; }
        public DbSet<RMAST> RMAST { get; set; }
        public DbSet<RMBUDGET> RMBUDGET { get; set; }
        public DbSet<RMCAR> RMCAR { get; set; }
        public DbSet<RMEQUIP> RMEQUIP { get; set; }
        public DbSet<RMFEEM> RMFEEM { get; set; }
        public DbSet<RMLIV> RMLIV { get; set; }
        public DbSet<RMNEWS> RMNEWS { get; set; }
        public DbSet<RMTEL> RMTEL { get; set; }
        public DbSet<ROOMF> ROOMF { get; set; }
        public DbSet<RTNFEE> RTNFEE { get; set; }
        public DbSet<UPDNOTE> UPDNOTE { get; set; }
        public DbSet<WATERFEE> WATERFEE { get; set; }
        public DbSet<YMBUDGET> YMBUDGET { get; set; }
        public DbSet<BONUS> BONUS { get; set; }
        public DbSet<EMPLOYEE> EMPLOYEE { get; set; }
        public DbSet<CONFCODE> CONFCODE { get; set; }
        public DbSet<YRCNT> YRCNT { get; set; }
        public DbSet<YRHCNT> YRHCNT { get; set; }
        public DbSet<V_ACCF> V_ACCF { get; set; }
        public DbSet<V_BROKER> V_BROKER { get; set; }
        public DbSet<V_CNEQUIP> V_CNEQUIP { get; set; }
        public DbSet<V_CNHIN> V_CNHIN { get; set; }
        public DbSet<V_CNHOUT> V_CNHOUT { get; set; }
        public DbSet<V_DEALER> V_DEALER { get; set; }
        public DbSet<V_RMCAR> V_RMCAR { get; set; }
        public DbSet<V_RMEQUIP> V_RMEQUIP { get; set; }
        public DbSet<V_RMLIVIN> V_RMLIVIN { get; set; }
        public DbSet<V_RMLIVOUT> V_RMLIVOUT { get; set; }
        public DbSet<V_RMTEL> V_RMTEL { get; set; }
        public DbSet<V_ROOM> V_ROOM { get; set; }
        public DbSet<view_RPT511> view_RPT511 { get; set; }
        public DbSet<view_RPT5112> view_RPT5112 { get; set; }
        public DbSet<view_RPT524> view_RPT524 { get; set; }
        public DbSet<view_RPT506> view_RPT506 { get; set; }
        public DbSet<SEQNUM> SEQNUM { get; set; }
        public DbSet<view_RPT508> view_RPT508 { get; set; }
        public DbSet<view_RPT512> view_RPT512 { get; set; }
        public DbSet<view_RPT515> view_RPT515 { get; set; }
        public DbSet<view_RPT516> view_RPT516 { get; set; }
        public DbSet<view_RPT525> view_RPT525 { get; set; }
        public DbSet<RPTCONF> RPTCONF { get; set; }
        public DbSet<view_RPT513> view_RPT513 { get; set; }
        public DbSet<ACCF> ACCF { get; set; }
        public DbSet<view_RPT502> view_RPT502 { get; set; }
        public DbSet<view_RPT501> view_RPT501 { get; set; }
        public DbSet<MKRPAY> MKRPAY { get; set; }
        public DbSet<FEEFORM> FEEFORM { get; set; }
        public DbSet<view_MKRPAY> view_MKRPAY { get; set; }
        public DbSet<OTHERFEE> OTHERFEE { get; set; }
    
        public virtual ObjectResult<Nullable<int>> ProcGetLastDay(string dt)
        {
            var dtParameter = dt != null ?
                new ObjectParameter("dt", dt) :
                new ObjectParameter("dt", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("ProcGetLastDay", dtParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> ProcGetMonthDiff(string sdt, string edt)
        {
            var sdtParameter = sdt != null ?
                new ObjectParameter("sdt", sdt) :
                new ObjectParameter("sdt", typeof(string));
    
            var edtParameter = edt != null ?
                new ObjectParameter("edt", edt) :
                new ObjectParameter("edt", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("ProcGetMonthDiff", sdtParameter, edtParameter);
        }
    
        public virtual ObjectResult<string> ProcGetTagid()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ProcGetTagid");
        }
    }
}
