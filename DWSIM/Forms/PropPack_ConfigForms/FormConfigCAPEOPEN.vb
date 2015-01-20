﻿Imports Microsoft.Win32
Imports DWSIM.DWSIM.SimulationObjects.UnitOps.Auxiliary.CapeOpen
Imports CapeOpen
Imports DWSIM.Interfaces
Imports DWSIM.DWSIM.SimulationObjects.PropertyPackages

<System.Serializable()> Public Class FormConfigCAPEOPEN

    Inherits FormConfigBase

    <System.NonSerialized()> Public _copp, _pptpl As Object
    Public _selts As CapeOpenUnitOpInfo
    Private _coobjects As New List(Of DWSIM.SimulationObjects.UnitOps.Auxiliary.CapeOpen.CapeOpenUnitOpInfo)
    Private _loaded As Boolean = False
    Public _coversion As String
    Public _ppname As String
    Public _mappings As New Dictionary(Of String, String)
    Public _phasemappings As New Dictionary(Of String, PhaseInfo)

    'CAPE-OPEN 1.0 Property Calculation Routines {678c09a2-7d66-11d2-a67d-00105a42887f}
    'CAPE-OPEN 1.0 Thermo Systems {678c09a3-7d66-11d2-a67d-00105a42887f}
    'CAPE-OPEN 1.0 Thermo Property Packages {678c09a4-7d66-11d2-a67d-00105a42887f}

    'CAPE-OPEN 1.1 Property Package {cf51e384-0110-4ed8-acb7-b50cfde6908e}
    'CAPE-OPEN 1.1 Physical Property Calculator {cf51e385-0110-4ed8-acb7-b50cfde6908e}
    'CAPE-OPEN 1.1 Equilibrium Calculator {cf51e386-0110-4ed8-acb7-b50cfde6908e}
    'CAPE-OPEN 1.1 Physical Property Package Managers {cf51e383-0110-4ed8-acb7-b50cfde6908e}

    Private Sub FormConfigCAPEOPEN_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me._coobjects.Clear()

        If _coversion = "1.0" Then
            SearchCO("{678c09a3-7d66-11d2-a67d-00105a42887f}")
            rb10.Checked = True
        Else
            SearchCO("{cf51e383-0110-4ed8-acb7-b50cfde6908e}")
            rb11.Checked = True
        End If

        Me.cbThermoServer.Items.Clear()

        For Each coui As CapeOpenUnitOpInfo In _coobjects
            With coui
                Me.cbThermoServer.Items.Add(.Name)
            End With
        Next

        If Not _selts Is Nothing Then
            cbThermoServer.SelectedItem = _selts.Name
            If Not _pptpl Is Nothing Then
                Dim t As Type = Type.GetTypeFromProgID(_selts.TypeName)
                _pptpl = Activator.CreateInstance(t)
            End If
            Dim proppacks As String()
            If _coversion = "1.0" Then
                proppacks = CType(_pptpl, ICapeThermoSystem).GetPropertyPackages
            Else
                proppacks = CType(_pptpl, ICapeThermoPropertyPackageManager).GetPropertyPackageList
            End If
            For Each pp As String In proppacks
                Me.cbPropPack.Items.Add(pp)
            Next
            Me.lblName2.Text = _selts.Name
            Me.lblVersion2.Text = _selts.Version
            Me.lblAuthorURL2.Text = _selts.VendorURL
            Me.lblDesc2.Text = _selts.Description
            Me.lblAbout2.Text = _selts.AboutInfo
        End If

        If Not _copp Is Nothing Then
            Dim pname As String = CType(_copp, ICapeIdentification).ComponentName
            If Not Me.cbPropPack.Items.Contains(pname) Then
                Me.cbPropPack.Items.Add(pname)
            End If
            cbPropPack.SelectedItem = pname
        End If

        UpdateMappings()

        _loaded = True

    End Sub

    Private Sub FormConfigCAPEOPEN_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If _loaded Then

            For Each r As DataGridViewRow In dgmap.Rows
                Dim comp As String = r.Cells(0).Value
                Dim map As String = r.Cells(2).Value
                _mappings(comp) = map
            Next

            For Each r As DataGridViewRow In dgvph.Rows
                Dim phase As String = r.Cells(0).Value
                Dim cophaselabel As String = r.Cells(2).Value
                _phasemappings(phase).PhaseLabel = cophaselabel
            Next

        End If

    End Sub

    Sub UpdateMappings()

        'compounds/components and phases

        If _mappings Is Nothing Then _mappings = New Dictionary(Of String, String)

        'check mappings
        Dim nc As Integer = My.Application.ActiveSimulation.Options.SelectedComponents.Count
        Dim mc As Integer = _mappings.Count
        Dim remap As Boolean = False
        If nc <> mc Then
            'remapping necessary
            remap = True
        Else
            For Each c As DWSIM.ClassesBasicasTermodinamica.ConstantProperties In My.Application.ActiveSimulation.Options.SelectedComponents.Values
                If Not _mappings.ContainsKey(c.Name) Then
                    'remapping necessary
                    remap = True
                End If
            Next
        End If

        If remap Then
            _mappings.Clear()
            For Each c As DWSIM.ClassesBasicasTermodinamica.ConstantProperties In My.Application.ActiveSimulation.Options.SelectedComponents.Values
                _mappings.Add(c.Name, "")
            Next
        End If

        Dim cb, cb2 As New DataGridViewComboBoxCell

        Dim complist As Object = Nothing
        Dim formulae As Object = Nothing
        Dim boiltemps As Object = Nothing
        Dim molwts As Object = Nothing
        Dim casids As Object = Nothing
        Dim names As Object = Nothing
        Dim plist As Object = Nothing
        Dim staggr As Object = Nothing
        Dim kci As Object = Nothing

        If Not _copp Is Nothing Then
            If _coversion = "1.0" Then
                CType(_copp, ICapeThermoPropertyPackage).GetComponentList(complist, formulae, names, boiltemps, molwts, casids)
                plist = CType(_copp, ICapeThermoPropertyPackage).GetPhaseList()
            Else
                CType(_copp, ICapeThermoCompounds).GetCompoundList(complist, formulae, names, boiltemps, molwts, casids)
                CType(_copp, ICapeThermoPhases).GetPhaseList(plist, staggr, kci)
            End If
            For Each s As String In complist
                cb.Items.Add(s)
            Next
            dgmap.Columns(2).CellTemplate = cb
            For Each s As String In complist
                If _mappings.ContainsKey(s) Then
                    If _mappings(s) = "" Then _mappings(s) = s
                End If
            Next
            Me.dgmap.Rows.Clear()
            For Each kvp As KeyValuePair(Of String, String) In _mappings
                Me.dgmap.Rows.Add(New Object() {kvp.Key, DWSIM.App.GetComponentName(kvp.Key), kvp.Value})
            Next

            cb2.Items.Add("")
            cb2.Items.Add("Disabled")
            For Each s As String In plist
                cb2.Items.Add(s)
            Next
            dgvph.Columns(2).CellTemplate = cb2
            Me.dgvph.Rows.Clear()
            For Each kvp As KeyValuePair(Of String, PhaseInfo) In _phasemappings
                Me.dgvph.Rows.Add(New Object() {kvp.Key, kvp.Key, kvp.Value.PhaseLabel})
            Next

        End If

    End Sub

    Sub SearchCO(ByVal CLSID As String)

        Dim keys As String() = My.Computer.Registry.ClassesRoot.OpenSubKey("CLSID", False).GetSubKeyNames()

        For Each k2 In keys
            Dim mykey As RegistryKey = My.Computer.Registry.ClassesRoot.OpenSubKey("CLSID", False).OpenSubKey(k2, False)
            For Each s As String In mykey.GetSubKeyNames()
                If s = "Implemented Categories" Then
                    Dim arr As Array = mykey.OpenSubKey("Implemented Categories").GetSubKeyNames()
                    For Each s2 As String In arr
                        If s2.ToLower = CLSID Then
                            'this is a CAPE-OPEN UO
                            Dim myuo As New DWSIM.SimulationObjects.UnitOps.Auxiliary.CapeOpen.CapeOpenUnitOpInfo
                            With myuo
                                .AboutInfo = mykey.OpenSubKey("CapeDescription").GetValue("About")
                                .CapeVersion = mykey.OpenSubKey("CapeDescription").GetValue("CapeVersion")
                                .Description = mykey.OpenSubKey("CapeDescription").GetValue("Description")
                                .HelpURL = mykey.OpenSubKey("CapeDescription").GetValue("HelpURL")
                                .Name = mykey.OpenSubKey("CapeDescription").GetValue("Name")
                                .VendorURL = mykey.OpenSubKey("CapeDescription").GetValue("VendorURL")
                                .Version = mykey.OpenSubKey("CapeDescription").GetValue("ComponentVersion")
                                .ImplementedCategory = CLSID
                                Try
                                    .TypeName = mykey.OpenSubKey("ProgID").GetValue("")
                                Catch ex As Exception
                                End Try
                                Try
                                    .Location = mykey.OpenSubKey("InProcServer32").GetValue("")
                                Catch ex As Exception
                                    .Location = mykey.OpenSubKey("LocalServer32").GetValue("")
                                End Try
                            End With
                            _coobjects.Add(myuo)
                        End If
                    Next
                End If
            Next
            mykey.Close()
        Next

    End Sub

    Private Sub cbPropServer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbThermoServer.SelectedIndexChanged

        If _loaded Then

            For Each coui As CapeOpenUnitOpInfo In _coobjects
                If coui.Name = cbThermoServer.SelectedItem.ToString Then
                    _selts = coui
                End If
            Next

            Dim t As Type = Type.GetTypeFromProgID(_selts.TypeName)
            _pptpl = Activator.CreateInstance(t)

            If TryCast(_pptpl, IPersistStreamInit) IsNot Nothing Then
                CType(_pptpl, IPersistStreamInit).InitNew()
            End If
            If TryCast(_pptpl, ICapeUtilities) IsNot Nothing Then
                CType(_pptpl, ICapeUtilities).Initialize()
            End If

            Dim proppacks As Object
            If _coversion = "1.0" Then
                proppacks = CType(_pptpl, ICapeThermoSystem).GetPropertyPackages
            Else
                proppacks = CType(_pptpl, ICapeThermoPropertyPackageManager).GetPropertyPackageList
            End If

            Me.cbPropPack.Items.Clear()
            If Not proppacks Is Nothing Then
                For Each pp As String In proppacks
                    Me.cbPropPack.Items.Add(pp)
                Next
            End If

            Me.lblName2.Text = _selts.Name
            Me.lblVersion2.Text = _selts.Version
            Me.lblAuthorURL2.Text = _selts.VendorURL
            Me.lblDesc2.Text = _selts.Description
            Me.lblAbout2.Text = _selts.AboutInfo

        End If

    End Sub

    Private Sub btnEditPropServer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditThermoServer.Click

        Dim myuo As ICapeUtilities = TryCast(_pptpl, ICapeUtilities)

        If Not myuo Is Nothing Then
            Try
                myuo.Edit()
                cbPropServer_SelectedIndexChanged(sender, e)
            Catch ex As Exception
                MessageBox.Show("Object is not editable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Object is not editable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub btnEditEqServer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditPropPack.Click

        Dim myuo As ICapeUtilities = TryCast(_copp, ICapeUtilities)

        If Not myuo Is Nothing Then
            Try
                myuo.Edit()
                UpdateMappings()
            Catch ex As Exception
                MessageBox.Show("Object is not editable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Object is not editable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub rb10_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rb10.CheckedChanged

        If rb10.Checked Then _coversion = "1.0" Else _coversion = "1.1"

        If _loaded Then

            Me._coobjects.Clear()

            If _coversion = "1.0" Then
                SearchCO("{678c09a3-7d66-11d2-a67d-00105a42887f}")
            Else
                SearchCO("{cf51e383-0110-4ed8-acb7-b50cfde6908e}")
            End If

            Me.cbThermoServer.Items.Clear()

            For Each coui As CapeOpenUnitOpInfo In _coobjects
                With coui
                    Me.cbThermoServer.Items.Add(.Name)
                End With
            Next

        End If

    End Sub

    Private Sub cbPropPack_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPropPack.SelectedIndexChanged

        If _loaded Then

            If _coversion = "1.0" Then
                _copp = CType(_pptpl, ICapeThermoSystem).ResolvePropertyPackage(cbPropPack.SelectedItem.ToString)
            Else
                _copp = CType(_pptpl, ICapeThermoPropertyPackageManager).GetPropertyPackage(cbPropPack.SelectedItem.ToString)
            End If
            If TryCast(_pptpl, IPersistStreamInit) IsNot Nothing Then
                CType(_copp, IPersistStreamInit).InitNew()
            End If
            If TryCast(_pptpl, ICapeUtilities) IsNot Nothing Then
                CType(_copp, ICapeUtilities).Initialize()
            End If
            _ppname = cbPropPack.SelectedItem.ToString
            UpdateMappings()
        End If

    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub dgmap_DataError(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgmap.DataError

    End Sub

    Private Sub dgvph_DataError(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgvph.DataError

    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

End Class